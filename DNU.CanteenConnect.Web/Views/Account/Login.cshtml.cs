using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DNU.CanteenConnect.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email hoặc Mã số SV/CB là bắt buộc.")]
            [Display(Name = "Email hoặc Mã số SV/CB")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [Display(Name = "Ghi nhớ đăng nhập")]
            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
            // Logic khi trang được tải lần đầu
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Đây là nơi bạn sẽ gọi API Back-end để xác thực đăng nhập
            // Ví dụ: var response = await _authService.Login(Input.Email, Input.Password);
            // Sau khi xác thực thành công, thiết lập cookie hoặc token.

            // Giả lập đăng nhập thành công
            if (Input.Email == "test@dnu.edu.vn" && Input.Password == "password")
            {
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToPage("/Index"); // Chuyển hướng về trang chủ
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                return Page();
            }
        }
    }
}