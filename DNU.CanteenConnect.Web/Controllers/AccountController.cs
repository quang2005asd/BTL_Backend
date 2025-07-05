using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace DNU.CanteenConnect.Web.Controllers
{
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
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
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
                var user = await _userManager.FindByEmailAsync(model.Email)
                           ?? await _userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Người dùng đã đăng nhập.");

                        // HỢP NHẤT GIỎ HÀNG
                        await MergeSessionCartToDbCart(user.Id);

                        TempData["SuccessMessage"] = "Đăng nhập thành công!";
                        return RedirectToLocal(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
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
                var userName = string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName;

                var user = new User
                {
                    UserName = userName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Người dùng đã tạo tài khoản mới với mật khẩu.");

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

        // HÀM HỢP NHẤT GIỎ HÀNG TỪ SESSION VÀO DB
        private async Task MergeSessionCartToDbCart(string userId)
        {
            var sessionCart = HttpContext.Session.Get<Cart>("GuestCart");
            if (sessionCart != null && sessionCart.CartItems.Any())
            {
                var dbCart = await _context.Carts
                                           .Include(c => c.CartItems)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

                if (dbCart == null)
                {
                    dbCart = new Cart { UserId = userId, CreatedDate = DateTime.Now };
                    _context.Carts.Add(dbCart);
                }

                foreach (var sessionItem in sessionCart.CartItems)
                {
                    var dbItem = dbCart.CartItems.FirstOrDefault(i => i.FoodItemId == sessionItem.FoodItemId);
                    if (dbItem != null)
                    {
                        dbItem.Quantity += sessionItem.Quantity;
                    }
                    else
                    {
                        dbCart.CartItems.Add(new CartItem
                        {
                            FoodItemId = sessionItem.FoodItemId,
                            Quantity = sessionItem.Quantity,
                            PriceAtAddition = sessionItem.PriceAtAddition
                        });
                    }
                }

                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("GuestCart");
            }
        }
    }
}
