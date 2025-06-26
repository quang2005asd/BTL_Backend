// DNU.CanteenConnect.Web/Pages/FoodItems/Details.cshtml.cs
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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa đối tượng FoodItem được hiển thị
        public FoodItem FoodItem { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Kiểm tra xem ID có được cung cấp không
            if (id == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không có ID
            }

            // Tìm món ăn trong database bằng ID, và bao gồm cả FoodCategory liên quan
            var fooditem = await _context.FoodItems
                .Include(f => f.FoodCategory) // Eager load FoodCategory
                .FirstOrDefaultAsync(m => m.ItemId == id);

            // Kiểm tra xem món ăn có tồn tại không
            if (fooditem == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy món ăn
            }

            // Gán món ăn tìm được vào thuộc tính FoodItem
            FoodItem = fooditem;
            return Page(); // Hiển thị trang
        }
    }
}