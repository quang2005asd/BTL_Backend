// DNU.CanteenConnect.Web/Controllers/AccountController.cs
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // For Data Annotations in ViewModels
using Microsoft.AspNetCore.Authorization; // For [AllowAnonymous] and [Authorize]
using System; // For DateTime

namespace DNU.CanteenConnect.Web.Controllers
{
    // Đây là nơi định nghĩa các ViewModels cho Login và Register.
    // Thường thì bạn sẽ đặt chúng trong thư mục 'ViewModels/Account'
    // nhưng để tiện cho một file, tôi sẽ định nghĩa ở đây.
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email hoặc mã số là bắt buộc.")]
        [Display(Name = "Email hoặc Mã số SV/CB")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = default!;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} và nhiều nhất {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = default!;

        [StringLength(255)]
        [Display(Name = "Tên người dùng (Tùy chọn)")]
        public string? UserName { get; set; }

        [StringLength(100)]
        [Display(Name = "Số điện thoại (Tùy chọn)")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }
    }

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger; // Thêm ILogger

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger) // Inject ILogger
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Kiểm tra xem người dùng có thể đăng nhập bằng Email hoặc UserName không
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Nếu không tìm thấy bằng email, thử tìm bằng UserName
                    user = await _userManager.FindByNameAsync(model.Email);
                }

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Người dùng đã đăng nhập.");
                        TempData["SuccessMessage"] = "Đăng nhập thành công!";
                        return RedirectToLocal(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        // Xử lý xác thực 2 yếu tố nếu có
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Tài khoản người dùng bị khóa.");
                        TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa.";
                        return RedirectToPage("./Lockout");
                    }
                }
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
            }
            // Nếu có lỗi, trả lại View với Model và lỗi
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Sử dụng Email làm UserName nếu UserName không được cung cấp, hoặc ngược lại
                var userName = string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName;

                var user = new User
                {
                    UserName = userName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.UtcNow // Gán ngày tạo tài khoản
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng đã tạo tài khoản mới với mật khẩu.");

                    // Gán vai trò "Customer" cho người dùng mới
                    await _userManager.AddToRoleAsync(user, "Customer");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("Người dùng đã tạo tài khoản và đăng nhập thành công.");
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
                    return RedirectToLocal(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    TempData["ErrorMessage"] = "Đăng ký không thành công: " + error.Description;
                }
            }
            // Nếu có lỗi, trả lại View với Model và lỗi
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Người dùng đã đăng xuất.");
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Điều hướng về trang chủ sau khi đăng xuất
                return RedirectToAction("Index", "Home");
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
