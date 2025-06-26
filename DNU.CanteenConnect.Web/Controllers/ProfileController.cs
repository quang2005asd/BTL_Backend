// DNU.CanteenConnect.Web/Controllers/ProfileController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize] // Yêu cầu người dùng phải đăng nhập để truy cập trang hồ sơ
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context; // Inject ApplicationDbContext

        public ProfileController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Profile/Index
        // Hiển thị thông tin hồ sơ của người dùng hiện tại
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Nếu không tìm thấy người dùng, chuyển hướng đến trang lỗi hoặc trang đăng nhập
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            return View(user);
        }

        // GET: /Profile/Edit
        // Hiển thị form để chỉnh sửa thông tin hồ sơ
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng để chỉnh sửa.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Tạo một ViewModel hoặc sử dụng trực tiếp User model nếu đủ đơn giản
            // Ở đây ta dùng User model trực tiếp vì không cần ẩn giấu nhiều thuộc tính
            return View(user);
        }

        // POST: /Profile/Edit
        // Xử lý việc cập nhật thông tin hồ sơ từ form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string userName, string email, string phoneNumber, string address)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Id != id) // Đảm bảo người dùng đang chỉnh sửa hồ sơ của chính họ
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa hồ sơ này.";
                return RedirectToAction("Index", "Profile");
            }

            // Cập nhật các thuộc tính của người dùng
            user.UserName = userName; // Lưu ý: Thay đổi UserName có thể ảnh hưởng đến đăng nhập
            user.Email = email;       // Cập nhật Email
            user.PhoneNumber = phoneNumber; // Cập nhật Số điện thoại
            user.Address = address; // Cập nhật địa chỉ

            // Cập nhật người dùng trong Identity system
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Thông tin hồ sơ đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Xử lý lỗi nếu có
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật hồ sơ. Vui lòng kiểm tra lại.";
                return View(user); // Trả về View với lỗi
            }
        }
    }
}
