// DNU.CanteenConnect.Web/Controllers/OrderController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Http; // Thêm using này cho HttpClient
using System.Text.Json; // Thêm using này cho JsonSerializer
using QRCoder; // Thêm using này cho thư viện QRCoder

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize] // Yêu cầu người dùng đăng nhập để tiến hành đặt hàng
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IStockAlertService _alertService;

        public OrderController(ApplicationDbContext context, UserManager<User> userManager, IStockAlertService alertService)
        {
            _context = context;
            _userManager = userManager;
            _alertService = alertService;
        }

        // GET: /Order/Checkout
        // Hiển thị trang thanh toán với thông tin giỏ hàng hiện tại
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Lấy giỏ hàng của người dùng hiện tại, bao gồm các món trong giỏ và thông tin chi tiết
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)! // Bao gồm các mục trong giỏ hàng
                                         .ThenInclude(ci => ci.FoodItem)! // Sau đó bao gồm thông tin FoodItem
                                             .ThenInclude(fi => fi.FoodCategory) // Bao gồm cả Category cho FoodItem
                                     .Include(c => c.CartItems)!
                                         .ThenInclude(ci => ci.FoodItem)!
                                             .ThenInclude(fi => fi.Canteen) // Bao gồm cả Canteen cho FoodItem
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            // Kiểm tra nếu giỏ hàng rỗng
            if (cart == null || !cart.CartItems!.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm món ăn trước khi thanh toán.";
                return RedirectToAction("Index", "Cart"); // Chuyển hướng về trang giỏ hàng nếu trống
            }

            // Lấy danh sách nhà ăn để người dùng chọn trong dropdown
            ViewBag.Canteens = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _context.Canteens.ToListAsync(), "CanteenId", "Name");

            return View(cart);
        }

        // POST: /Order/PlaceOrder
        // Xử lý việc đặt hàng từ giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công giả mạo yêu cầu từ trang web khác (CSRF)
        public async Task<IActionResult> PlaceOrder(int selectedCanteenId, string? notes, string paymentMethod) // Đã thêm tham số paymentMethod
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đặt hàng." });
            }

            // Lấy giỏ hàng của người dùng
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)!
                                         .ThenInclude(ci => ci.FoodItem) // Bao gồm thông tin FoodItem để lấy giá và các chi tiết khác
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
            }

            // Xác thực nhà ăn được chọn
            var selectedCanteen = await _context.Canteens.FindAsync(selectedCanteenId);
            if (selectedCanteen == null)
            {
                return Json(new { success = false, message = "Nhà ăn được chọn không hợp lệ." });
            }

            // Tạo một đối tượng Order mới từ thông tin giỏ hàng
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Notes = notes,
                CanteenId = selectedCanteenId,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.PriceAtAddition),
                PaymentMethod = paymentMethod // Lưu phương thức thanh toán
            };

            // Cập nhật trạng thái dựa trên phương thức thanh toán
            if (paymentMethod == "BankTransfer")
            {
                order.Status = "AwaitingPaymentConfirmation"; // Trạng thái cho chuyển khoản ngân hàng
            }
            else // Mặc định là CashOnDelivery hoặc các phương thức khác
            {
                order.Status = "Pending"; // Đang chờ xử lý
            }

            _context.Orders.Add(order); // Thêm đơn hàng vào DbContext
            await _context.SaveChangesAsync(); // Lưu để có OrderId được tạo bởi database

            // Tạo OrderItems từ CartItems
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId, // Liên kết với Order vừa tạo
                    ItemId = cartItem.FoodItemId,
                    Quantity = cartItem.Quantity,
                    PriceAtOrder = cartItem.PriceAtAddition // Lưu giá tại thời điểm đặt hàng để đảm bảo tính chính xác lịch sử
                };
                _context.OrderItems.Add(orderItem); // Thêm mục đơn hàng vào DbContext

                // --- DECREMENT STOCK QUANTITY ---
                var foodItem = await _context.FoodItems.FindAsync(cartItem.FoodItemId);
                if (foodItem != null)
                {
                    foodItem.StockQuantity -= cartItem.Quantity;
                    if (foodItem.StockQuantity < 0) foodItem.StockQuantity = 0; // Ensure stock doesn't go negative
                    
                    // Check and create stock alert if needed
                    await _alertService.CheckAndCreateAlertAsync(cartItem.FoodItemId, selectedCanteenId, foodItem.StockQuantity);
                }
            }

            // Xóa các món ăn trong giỏ hàng sau khi đã đặt hàng thành công
            _context.CartItems.RemoveRange(cart.CartItems); // Xóa tất cả CartItems của giỏ hàng này
            _context.Carts.Remove(cart); // Xóa luôn giỏ hàng sau khi chuyển đổi thành đơn hàng
            await _context.SaveChangesAsync(); // Lưu các thay đổi cuối cùng

            // Trả về kết quả thành công
            return Json(new { success = true, message = "Đơn hàng của bạn đã được đặt thành công!", redirectUrl = Url.Action("Details", "OrderHistory", new { id = order.OrderId }) });
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetQrCodeForCheckout(decimal totalAmount, string userName)
        {
            int cleanAmount = (int)Math.Round(totalAmount);

            // Thông tin ngân hàng cố định
            const string mbBankBin = "970422"; // BIN của MBBank theo API VietQR.io
            const string accountNumber = "278909999";
            const string accountName = "NGUYEN VIET QUANG"; // Tên tài khoản nhận tiền

            string vietQrApiUrl = "https://api.vietqr.io/v2/generate";

            // Nội dung chuyển khoản sẽ là generic vì chưa có OrderId
            string transferContent = $"DNU CANTEEN - {userName} - {cleanAmount} VNĐ";

            // Tạo đối tượng JSON để gửi đi
            var requestBody = new
            {
                acqId = mbBankBin,
                accountNo = accountNumber,
                amount = cleanAmount,
                addInfo = transferContent,
                accountName = accountName,
                template = "compact2" // Có thể dùng "compact", "qr_code", "print"
            };

            // Chuyển đổi đối tượng sang chuỗi JSON
            string jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Gửi yêu cầu POST đến API VietQR.io
                    HttpResponseMessage response = await client.PostAsync(vietQrApiUrl, httpContent);
                    response.EnsureSuccessStatusCode(); // Ném ngoại lệ nếu mã trạng thái là lỗi (4xx hoặc 5xx)

                    string responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument doc = JsonDocument.Parse(responseBody);
                    JsonElement root = doc.RootElement;

                    string? actualQrCodeData = null;
                    if (root.TryGetProperty("data", out JsonElement dataElement) &&
                        dataElement.TryGetProperty("qrCode", out JsonElement qrCodeElement))
                    {
                        actualQrCodeData = qrCodeElement.GetString();
                    }

                    if (string.IsNullOrEmpty(actualQrCodeData))
                    {
                        return BadRequest("Không thể lấy dữ liệu QR code hợp lệ từ API VietQR.");
                    }

                    // Sử dụng dữ liệu QR code thực tế (chuỗi EMVCo) để tạo ảnh bằng QRCoder
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        // ECCLevel.Q hoặc .H cung cấp khả năng sửa lỗi tốt hơn, giúp quét dễ hơn
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(actualQrCodeData, QRCodeGenerator.ECCLevel.Q);
                        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                        {
                            // Tạo ảnh PNG. Kích thước 20px mỗi module tạo ra ảnh đủ lớn để quét
                            byte[] qrCodeBytes = qrCode.GetGraphic(20);
                            return File(qrCodeBytes, "image/png"); // Trả về hình ảnh QR code
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Lỗi khi gọi API VietQR: {e.Message}");
                    return StatusCode(500, $"Lỗi server nội bộ khi tạo mã QR: {e.Message}. Vui lòng kiểm tra kết nối API VietQR.io.");
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"Lỗi khi phân tích phản hồi API VietQR: {e.Message}");
                    return StatusCode(500, $"Lỗi server nội bộ khi phân tích phản hồi QR code: {e.Message}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Đã xảy ra lỗi không mong muốn: {e.Message}");
                    return StatusCode(500, $"Đã xảy ra lỗi không mong muốn khi tạo mã QR: {e.Message}.");
                }
            }
        }
        
        // NEW ACTION: POST /Order/ConfirmBankTransferPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBankTransferPayment(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để xác nhận chuyển khoản.", newStatusDisplayName = "Lỗi" });
            }

            var order = await _context.Orders
                                    .Include(o => o.User) // Include User to get UserName for display purposes
                                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại hoặc không thuộc về bạn.", newStatusDisplayName = "Không tìm thấy" });
            }

            // Only allow confirmation if the order is awaiting payment confirmation and is a bank transfer
            if (order.PaymentMethod == "BankTransfer" && order.Status == "AwaitingPaymentConfirmation")
            {
                order.Status = "PaymentSubmitted"; // Update status to 'PaymentSubmitted'
                await _context.SaveChangesAsync();
                
                // Return the display name for the new status
                return Json(new { success = true, message = "Bạn đã xác nhận chuyển khoản. Đơn hàng đang chờ quản trị viên xác nhận.", newStatusDisplayName = DNU.CanteenConnect.Web.Controllers.OrderHistoryController.GetStatusDisplayName(order.Status) });
            }
            else
            {
                // If the current status is already PaymentSubmitted or other invalid status
                return Json(new { success = false, message = $"Trạng thái đơn hàng hiện tại là '{DNU.CanteenConnect.Web.Controllers.OrderHistoryController.GetStatusDisplayName(order.Status)}'. Không thể xác nhận chuyển khoản lúc này.", newStatusDisplayName = DNU.CanteenConnect.Web.Controllers.OrderHistoryController.GetStatusDisplayName(order.Status) });
            }
        }
    }
}
