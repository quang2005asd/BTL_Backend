using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize] // Yêu cầu người dùng phải đăng nhập để sử dụng các chức năng này
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager; // Dịch vụ để quản lý người dùng

        // Cập nhật constructor để nhận UserManager
        public ReviewsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: /Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,Rating,Comment")] Review review, string returnUrl)
        {
            // Lấy ID của người dùng đang đăng nhập từ thông tin phiên
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Trường hợp này hiếm khi xảy ra do đã có [Authorize]
                // nhưng vẫn là một bước kiểm tra an toàn
                return Challenge(); // Yêu cầu đăng nhập lại
            }

            // Gán các thông tin cần thiết một cách tự động
            review.UserId = userId; // <-- GÁN ID NGƯỜI DÙNG HIỆN TẠI
            review.ReviewDate = DateTime.Now;

            // Xóa các lỗi validation không cần thiết vì chúng không được gửi từ form
            ModelState.Remove("User");
            ModelState.Remove("FoodItem");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Gửi đánh giá thành công!";

                // Chuyển hướng người dùng trở lại trang mà họ vừa đánh giá
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    // Nếu có lỗi, chuyển về trang chủ như một giải pháp an toàn
                    return RedirectToAction("Index", "Home");
                }
            }
            
            // Nếu ModelState không hợp lệ (ví dụ: người dùng không cho điểm)
            TempData["ErrorMessage"] = "Gửi đánh giá không thành công. Vui lòng kiểm tra lại.";
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}