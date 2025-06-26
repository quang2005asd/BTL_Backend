// DNU.CanteenConnect.Web/Pages/FoodCategories/Details.cshtml.cs
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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa đối tượng FoodCategory được hiển thị
        public FoodCategory FoodCategory { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Kiểm tra xem ID có được cung cấp không
            if (id == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không có ID
            }

            // Tìm danh mục món ăn trong database bằng ID
            var foodcategory = await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);

            // Kiểm tra xem danh mục có tồn tại không
            if (foodcategory == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy danh mục
            }

            // Gán danh mục tìm được vào thuộc tính FoodCategory
            FoodCategory = foodcategory;
            return Page(); // Hiển thị trang
        }
    }
}