// DNU.CanteenConnect.Web/Pages/FoodItems/Create.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using System.Linq; // Required for OrderBy and ToListAsync
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync

namespace DNU.CanteenConnect.Web.Pages.FoodItems
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

        // Thuộc tính để chứa đối tượng FoodItem cho việc binding từ form
        [BindProperty]
        public FoodItem FoodItem { get; set; } = default!;

        // SelectList cho Danh mục Món ăn
        public SelectList FoodCategoryOptions { get; set; } = default!;

        // SelectList cho Nhà ăn
        public SelectList CanteenOptions { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu (GET request)
        public async Task<IActionResult> OnGetAsync()
        {
            // Điền dữ liệu cho dropdown Danh mục
            FoodCategoryOptions = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name");
            
            // Điền dữ liệu cho dropdown Nhà ăn
            // Lấy danh sách các nhà ăn từ database và sắp xếp theo tên
            CanteenOptions = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name");

            // Đặt mặc định IsAvailable và IsSpecialOfTheDay
            FoodItem = new FoodItem { IsAvailable = true, IsSpecialOfTheDay = false };

            return Page();
        }

        // Phương thức được gọi khi form được gửi (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra ModelState.IsValid trước để đảm bảo dữ liệu hợp lệ
            if (!ModelState.IsValid)
            {
                // Nếu không hợp lệ, điền lại các tùy chọn và hiển thị trang
                // Cần điền lại CanteenOptions nếu ModelState.IsValid là false
                FoodCategoryOptions = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name");
                CanteenOptions = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name");
                return Page();
            }

            _context.FoodItems.Add(FoodItem);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
