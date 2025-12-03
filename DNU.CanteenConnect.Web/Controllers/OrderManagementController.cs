// DNU.CanteenConnect.Web/Controllers/OrderManagementController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Helpers; // <-- THÊM USING NÀY
using DNU.CanteenConnect.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class OrderManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderManagementController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // --- CÁC PHẦN SAU ĐƯỢC GIỮ NGUYÊN 100% TỪ CODE CỦA BẠN ---
        private static readonly Dictionary<string, string> _statusDisplayNames = new Dictionary<string, string>
        {
            { "Pending", "Đang chờ xử lý" }, { "AwaitingPaymentConfirmation", "Chờ khách hàng xác nhận CK" },
            { "PaymentSubmitted", "Khách hàng đã xác nhận CK" }, { "Paid", "Đã thanh toán" },
            { "Preparing", "Đang chuẩn bị" }, { "Ready", "Đã sẵn sàng" },
            { "Completed", "Đã hoàn thành" }, { "Cancelled", "Đã hủy" }
        };
        
        public static string GetStatusDisplayName(string status) => _statusDisplayNames.GetValueOrDefault(status, status);

        // --- ACTION INDEX ĐÃ ĐƯỢC NÂNG CẤP ĐỂ PHÂN TRANG ---
        public async Task<IActionResult> Index(string searchString, string statusFilter, int? pageNumber)
        {
            var ordersQuery = _context.Orders
                                    .Include(o => o.User)
                                    .Include(o => o.Canteen)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out int orderId)) {
                    ordersQuery = ordersQuery.Where(o => o.OrderId == orderId);
                } else {
                    ordersQuery = ordersQuery.Where(o => o.User!.UserName.Contains(searchString));
                }
            }

            if (!string.IsNullOrEmpty(statusFilter)) {
                ordersQuery = ordersQuery.Where(o => o.Status == statusFilter);
            }

            ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);

            int pageSize = 25;
            var paginatedOrders = await PaginatedList<Order>.CreateAsync(ordersQuery, pageNumber ?? 1, pageSize);

            var statusOptions = _statusDisplayNames.Select(kv => new SelectListItem { Value = kv.Key, Text = kv.Value }).ToList();

            var viewModel = new OrderManagementViewModel
            {
                Orders = paginatedOrders,
                StatusFilterOptions = new SelectList(statusOptions, "Value", "Text", statusFilter),
                CurrentStatusFilter = statusFilter,
                CurrentSearchString = searchString
            };

            return View(viewModel);
        }
        
        // --- GIỮ NGUYÊN HOÀN TOÀN ACTION DETAILS CỦA BẠN ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders
                                    .Include(o => o.User)
                                    .Include(o => o.Canteen)
                                    .Include(o => o.OrderItems)!
                                        .ThenInclude(oi => oi.FoodItem)!
                                            .ThenInclude(fi => fi.FoodCategory)
                                    .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null) return NotFound();
            var availableStatuses = new List<string> { "Pending", "AwaitingPaymentConfirmation", "PaymentSubmitted", "Paid", "Preparing", "Ready", "Completed", "Cancelled" };
            ViewBag.Statuses = new SelectList(availableStatuses.Select(s => new { Value = s, Text = GetStatusDisplayName(s) }), "Value", "Text", order.Status);
            return View(order);
        }

        // --- GIỮ NGUYÊN HOÀN TOÀN ACTION UPDATESTATUS CỦA BẠN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }
            var validStatuses = new List<string> { "Pending", "AwaitingPaymentConfirmation", "PaymentSubmitted", "Paid", "Preparing", "Ready", "Completed", "Cancelled" };
            if (!validStatuses.Contains(newStatus)) {
                TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }
            order.Status = newStatus;
            await _context.SaveChangesAsync();
            
            // --- GỬI THÔNG BÁO REAL-TIME BẰNG SIGNALR ---
            string displayName = GetStatusDisplayName(newStatus);
            await _hubContext.Clients.User(order.UserId).SendAsync("ReceiveOrderStatusUpdate", orderId, newStatus, displayName);
            
            // --- LƯU THÔNG BÁO VÀO DATABASE ---
            var notification = new Notification
            {
                UserId = order.UserId,
                OrderId = orderId,
                Message = $"Đơn hàng #{orderId} đã được cập nhật thành '{displayName}'",
                NotificationType = "OrderStatusUpdate",
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{orderId} thành '{displayName}'.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        // --- THÊM ACTION MỚI ĐỂ XÁC NHẬN THANH TOÁN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBankPayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            if (order.PaymentMethod == "BankTransfer" && order.Status == "PaymentSubmitted")
            {
                order.Status = "Paid"; // Chuyển trạng thái thành "Đã thanh toán"
                await _context.SaveChangesAsync();
                
                // --- GỬI THÔNG BÁO REAL-TIME BẰNG SIGNALR ---
                string displayName = GetStatusDisplayName("Paid");
                await _hubContext.Clients.User(order.UserId).SendAsync("ReceiveOrderStatusUpdate", orderId, "Paid", displayName);
                
                // --- LƯU THÔNG BÁO VÀO DATABASE ---
                var notification = new Notification
                {
                    UserId = order.UserId,
                    OrderId = orderId,
                    Message = $"Thanh toán cho đơn hàng #{orderId} đã được xác nhận",
                    NotificationType = "PaymentConfirmed",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Đã xác nhận thanh toán cho đơn hàng #{orderId}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xác nhận thanh toán cho đơn hàng này.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}