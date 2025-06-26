// DNU.CanteenConnect.Web/Controllers/OrderHistoryController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using QRCoder;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize]
    public class OrderHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderHistoryController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private static readonly Dictionary<string, string> _statusDisplayNames = new Dictionary<string, string>
        {
            { "Pending", "Đang chờ xử lý" },
            { "AwaitingPaymentConfirmation", "Chờ khách hàng xác nhận CK" },
            { "PaymentSubmitted", "Khách hàng đã xác nhận CK" },
            { "Paid", "Đã thanh toán" },
            { "Preparing", "Đang chuẩn bị" },
            { "Ready", "Đã sẵn sàng" },
            { "Completed", "Đã hoàn thành" },
            { "Cancelled", "Đã hủy" }
        };

        public static string GetStatusDisplayName(string status)
        {
            return _statusDisplayNames.GetValueOrDefault(status, status);
        }

        // --- ACTION INDEX CỦA BẠN ĐƯỢC GIỮ NGUYÊN ---
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var orders = await _context.Orders
                                        .Include(o => o.OrderItems)!
                                            .ThenInclude(oi => oi.FoodItem)!
                                                .ThenInclude(fi => fi.FoodCategory)
                                        .Include(o => o.Canteen)
                                        .Where(o => o.UserId == userId)
                                        .OrderByDescending(o => o.OrderDate)
                                        .ToListAsync();
            return View(orders);
        }

        // --- ACTION DETAILS ĐÃ ĐƯỢC NÂNG CẤP ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var order = await _context.Orders
                                    .Include(o => o.OrderItems)!
                                        .ThenInclude(oi => oi.FoodItem)
                                    .Include(o => o.Canteen)
                                    .Include(o => o.User)
                                    .FirstOrDefaultAsync(m => m.OrderId == id && m.UserId == userId);

            if (order == null) return NotFound();

            // Lấy danh sách các ID của món ăn mà người dùng này đã đánh giá
            var userReviewedItemIds = await _context.Reviews
                                                    .Where(r => r.UserId == userId)
                                                    .Select(r => r.ItemId)
                                                    .Distinct()
                                                    .ToListAsync();

            // Tạo ViewModel để gửi ra View
            var viewModel = new OrderHistoryDetailViewModel
            {
                Order = order,
                ItemReviewStatuses = order.OrderItems.Select(oi => new ReviewedItemStatus
                {
                    OrderItem = oi,
                    HasBeenReviewed = userReviewedItemIds.Contains(oi.ItemId)
                }).ToList()
            };

            return View(viewModel);
        }

        // --- ACTION GetQrCode CỦA BẠN ĐƯỢC GIỮ NGUYÊN ---
        [HttpGet]
        public async Task<IActionResult> GetQrCode(int orderId)
        {
            var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng để tạo mã QR.");
            var totalAmount = (int)order.TotalAmount;
            var userName = order.User?.UserName ?? "N/A";
            var transferContent = $"DNU CANTEEN - HD #{order.OrderId} - {userName}";
            const string mbBankBin = "970422";
            const string accountNumber = "278909999";
            const string accountName = "NGUYEN VIET QUANG";
            string vietQrApiUrl = "https://api.vietqr.io/v2/generate";
            var requestBody = new { acqId = mbBankBin, accountNo = accountNumber, amount = totalAmount, addInfo = transferContent, accountName = accountName, template = "compact2" };
            string jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync(vietQrApiUrl, httpContent);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument doc = JsonDocument.Parse(responseBody);
                    JsonElement root = doc.RootElement;
                    string actualQrCodeData = "";
                    if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.TryGetProperty("qrCode", out JsonElement qrCodeElement))
                    {
                        actualQrCodeData = qrCodeElement.GetString();
                    }
                    if (string.IsNullOrEmpty(actualQrCodeData)) return BadRequest("Không thể lấy dữ liệu QR code hợp lệ từ API VietQR.");
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(actualQrCodeData, QRCodeGenerator.ECCLevel.Q);
                        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                        {
                            byte[] qrCodeBytes = qrCode.GetGraphic(20);
                            return File(qrCodeBytes, "image/png");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Lỗi khi tạo mã QR: {e.Message}");
                    return StatusCode(500, "Lỗi server nội bộ khi tạo mã QR.");
                }
            }
        }

        // --- ACTION ConfirmPayment CỦA BẠN ĐƯỢC GIỮ NGUYÊN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Json(new { success = false, message = "Bạn cần đăng nhập để xác nhận thanh toán." });
            
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
            if (order == null) return Json(new { success = false, message = "Đơn hàng không tồn tại hoặc không thuộc về bạn." });
            
            if (order.PaymentMethod == "BankTransfer" && order.Status == "AwaitingPaymentConfirmation")
            {
                order.Status = "PaymentSubmitted";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Bạn đã xác nhận chuyển khoản. Đơn hàng đang chờ quản trị viên xác nhận thanh toán.", newStatusDisplayName = GetStatusDisplayName(order.Status) });
            }
            else if (order.PaymentMethod != "BankTransfer")
            {
                 return Json(new { success = false, message = "Phương thức thanh toán không phải chuyển khoản ngân hàng." });
            }
            else
            {
                return Json(new { success = false, message = $"Trạng thái đơn hàng không cho phép xác nhận chuyển khoản lúc này. Trạng thái hiện tại: {GetStatusDisplayName(order.Status)}", newStatusDisplayName = GetStatusDisplayName(order.Status) });
            }
        }
    }
}