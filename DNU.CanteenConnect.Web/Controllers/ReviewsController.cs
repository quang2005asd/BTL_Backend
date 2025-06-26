using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
// Thêm using này nếu bạn dùng Microsoft Identity để quản lý người dùng
// using Microsoft.AspNetCore.Identity; 
// using System.Security.Claims;

namespace DNU.CanteenConnect.Web.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        // private readonly UserManager<User> _userManager; // Bỏ comment nếu dùng Identity

        public ReviewsController(ApplicationDbContext context /*, UserManager<User> userManager */)
        {
            _context = context;
            // _userManager = userManager; // Bỏ comment nếu dùng Identity
        }

        // POST: /Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Tham số 'returnUrl' giúp chúng ta biết quay trở lại trang nào sau khi gửi đánh giá
        public async Task<IActionResult> Create(Review review, string returnUrl)
        {
            // Giả sử người dùng đã đăng nhập.
            // Đoạn code lấy UserId sẽ thay đổi tùy theo cách bạn quản lý User (Identity hay tự làm)
            // CÁCH 1: Dùng M. Identity (khuyên dùng)
            // review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // CÁCH 2: Tạm thời hard-code để test
            review.UserId = "98fd1302-d9b9-41b4-8f68-404233352def"; // << Sẽ sửa lại sau khi có chức năng đăng nhập

            review.ReviewDate = DateTime.Now;

            // Xóa lỗi của các trường không được bind từ form để ModelState hợp lệ
            ModelState.Remove("User");
            ModelState.Remove("FoodItem");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();

                // Chuyển hướng người dùng trở lại trang chi tiết món ăn mà họ vừa đánh giá
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    // Nếu returnUrl không hợp lệ, chuyển về trang chủ
                    return RedirectToAction("Index", "Home");
                }
            }
            
            // Nếu có lỗi, chúng ta cần xử lý (sẽ làm ở bước sau)
            // Tạm thời chỉ chuyển hướng về trang cũ
             if (Url.IsLocalUrl(returnUrl))
            {
                TempData["ErrorMessage"] = "Gửi đánh giá không thành công. Vui lòng thử lại.";
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}