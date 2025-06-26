// DNU.CanteenConnect.Web/Pages/FoodCategories/Delete.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodCategories
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

        // Thuộc tính để chứa đối tượng FoodCategory được hiển thị
        [BindProperty] // Cần BindProperty cho OnPost
        public FoodCategory FoodCategory { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID (GET request)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm danh mục món ăn trong database bằng ID
            var foodcategory = await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);

            if (foodcategory == null)
            {
                return NotFound();
            }

            FoodCategory = foodcategory;
            return Page();
        }

        // Phương thức được gọi khi người dùng xác nhận xóa (POST request)
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm danh mục món ăn trong database bằng ID
            var foodcategory = await _context.FoodCategories.FindAsync(id);

            if (foodcategory != null)
            {
                FoodCategory = foodcategory;
                // Xóa đối tượng FoodCategory khỏi DbSet FoodCategories
                _context.FoodCategories.Remove(FoodCategory);
                // Lưu các thay đổi vào database
                await _context.SaveChangesAsync();
            }

            // Chuyển hướng về trang Index (danh sách danh mục món ăn) sau khi xóa thành công
            return RedirectToPage("./Index");
        }
    }
}
