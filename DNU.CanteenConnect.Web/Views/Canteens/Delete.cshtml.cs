// DNU.CanteenConnect.Web/Pages/Canteens/Delete.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.Canteens
{
    // Yêu cầu vai trò Admin hoặc CanteenStaff để truy cập trang này
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa đối tượng Canteen được hiển thị
        [BindProperty] // Cần BindProperty cho OnPost
        public Canteen Canteen { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID (GET request)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm nhà ăn trong database bằng ID
            var canteen = await _context.Canteens.FirstOrDefaultAsync(m => m.CanteenId == id);

            if (canteen == null)
            {
                return NotFound();
            }

            Canteen = canteen;
            return Page();
        }

        // Phương thức được gọi khi người dùng xác nhận xóa (POST request)
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm nhà ăn trong database bằng ID
            var canteen = await _context.Canteens.FindAsync(id);

            if (canteen != null)
            {
                Canteen = canteen;
                // Xóa đối tượng Canteen khỏi DbSet Canteens
                _context.Canteens.Remove(Canteen);
                // Lưu các thay đổi vào database
                await _context.SaveChangesAsync();
            }

            // Chuyển hướng về trang Index (danh sách nhà ăn) sau khi xóa thành công
            return RedirectToPage("./Index");
        }
    }
}
