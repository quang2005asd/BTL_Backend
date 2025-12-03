using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Notification/History
        // Hiển thị lịch sử thông báo của người dùng hiện tại
        public async Task<IActionResult> History(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            const int pageSize = 20;
            
            // Lấy tất cả thông báo của người dùng, sorted by newest first
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .Include(n => n.Order)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize + 1)
                .ToListAsync();

            var totalCount = await _context.Notifications
                .Where(n => n.UserId == userId)
                .CountAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (totalCount + pageSize - 1) / pageSize;
            ViewBag.TotalNotifications = totalCount;
            ViewBag.UnreadCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return View(notifications);
        }

        // POST: /Notification/MarkAsRead/{id}
        // Đánh dấu một thông báo đã đọc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notification == null)
            {
                return Json(new { success = false, message = "Thông báo không tồn tại." });
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã đánh dấu thông báo đã đọc." });
        }

        // POST: /Notification/MarkAllAsRead
        // Đánh dấu tất cả thông báo đã đọc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã đánh dấu {unreadNotifications.Count} thông báo đã đọc." });
        }

        // GET: /Notification/GetUnreadCount
        // Lấy số lượng thông báo chưa đọc (for AJAX)
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var unreadCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return Json(new { count = unreadCount });
        }

        // GET: /Notification/GetLatest
        // Lấy thông báo mới nhất (for AJAX - show in dropdown)
        public async Task<IActionResult> GetLatest(int count = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { notifications = new List<object>() });
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .Select(n => new
                {
                    n.NotificationId,
                    n.Message,
                    n.NotificationType,
                    n.CreatedAt,
                    n.IsRead,
                    OrderId = n.OrderId ?? 0
                })
                .ToListAsync();

            return Json(new { notifications = notifications });
        }
    }
}
