// DNU.CanteenConnect.Web/Pages/FoodItems/Delete.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodItems
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

        // Thuộc tính để chứa đối tượng FoodItem được hiển thị
        [BindProperty] // Cần BindProperty cho OnPost
        public FoodItem FoodItem { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID (GET request)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm món ăn trong database bằng ID, bao gồm cả FoodCategory
            var fooditem = await _context.FoodItems
                                        .Include(f => f.FoodCategory) // Eager load FoodCategory
                                        .FirstOrDefaultAsync(m => m.ItemId == id);

            if (fooditem == null)
            {
                return NotFound();
            }

            FoodItem = fooditem;
            return Page();
        }

        // Phương thức được gọi khi người dùng xác nhận xóa (POST request)
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm món ăn trong database bằng ID
            var fooditem = await _context.FoodItems.FindAsync(id);

            if (fooditem != null)
            {
                FoodItem = fooditem;
                // Xóa đối tượng FoodItem khỏi DbSet FoodItems
                _context.FoodItems.Remove(FoodItem);
                // Lưu các thay đổi vào database
                await _context.SaveChangesAsync();
            }

            // Chuyển hướng về trang Index (danh sách món ăn) sau khi xóa thành công
            return RedirectToPage("./Index");
        }
    }
}
