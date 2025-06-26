using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager; // <-- THÊM VÀO

        // Cập nhật constructor để nhận RoleManager
        public UserManagementController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager; // <-- THÊM VÀO
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            // ... Action Index của bạn giữ nguyên như cũ ...
            int pageSize = 15;
            var userViewModels = new List<UserViewModel>();
            var users = await _context.Users.OrderBy(u => u.UserName).ToListAsync();

            foreach (var user in users)
            {
                userViewModels.Add(new UserViewModel
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            
            var paginatedUsers = PaginatedList<UserViewModel>.Create(userViewModels, pageNumber ?? 1, pageSize);
            return View(paginatedUsers);
        }

        // --- THÊM ACTION MỚI ĐỂ HIỂN THỊ TRANG QUẢN LÝ VAI TRÒ ---
        // GET: /UserManagement/ManageRoles/some-user-id
        [HttpGet]
        public async Task<IActionResult> ManageRoles(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Người dùng với ID = {id} không tồn tại.";
                return View("NotFound"); // Hoặc một trang lỗi tùy chỉnh
            }

            var viewModel = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            // Lấy tất cả các vai trò và kiểm tra xem người dùng có vai trò đó không
            foreach (var role in _roleManager.Roles.ToList())
            {
                viewModel.Roles.Add(new RoleSelection
                {
                    RoleName = role.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(viewModel);
        }

        // --- THÊM ACTION MỚI ĐỂ XỬ LÝ VIỆC CẬP NHẬT VAI TRÒ ---
        // POST: /UserManagement/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Người dùng với ID = {model.UserId} không tồn tại.";
                return View("NotFound");
            }

            // Lấy các vai trò hiện tại của người dùng
            var userRoles = await _userManager.GetRolesAsync(user);
            // Lấy các vai trò được chọn từ form
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName);

            // Xóa các vai trò cũ không được chọn nữa
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa các vai trò cũ của người dùng.");
                return View(model); // Trở lại form với thông báo lỗi
            }

            // Thêm các vai trò mới được chọn
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Không thể thêm các vai trò mới cho người dùng.");
                return View(model); // Trở lại form với thông báo lỗi
            }

            TempData["SuccessMessage"] = $"Đã cập nhật vai trò cho người dùng {user.UserName} thành công.";
            return RedirectToAction("Index");
        }
    }
}