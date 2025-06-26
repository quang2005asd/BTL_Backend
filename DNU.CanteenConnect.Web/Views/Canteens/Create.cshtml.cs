// DNU.CanteenConnect.Web/Pages/Canteens/Create.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.Canteens
{
    // Yêu cầu vai trò Admin hoặc CanteenStaff để truy cập trang này
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Phương thức này được gọi khi trang được tải lần đầu (GET request)
        public IActionResult OnGet()
        {
            // Không cần làm gì đặc biệt, chỉ hiển thị form trống
            return Page();
        }

        // Thuộc tính để nhận dữ liệu từ form (khi người dùng POST request)
        [BindProperty]
        public Canteen Canteen { get; set; } = default!;

        // Phương thức này được gọi khi form được gửi đi (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra xem dữ liệu từ form có hợp lệ theo các Data Annotations trong model Canteen không
            if (!ModelState.IsValid)
            {
                // Nếu không hợp lệ, hiển thị lại form với các thông báo lỗi
                return Page();
            }

            // Thêm đối tượng Canteen mới vào DbSet Canteens
            _context.Canteens.Add(Canteen);
            // Lưu các thay đổi vào database
            await _context.SaveChangesAsync();

            // Chuyển hướng về trang Index (danh sách nhà ăn) sau khi thêm thành công
            return RedirectToPage("./Index");
        }
    }
}