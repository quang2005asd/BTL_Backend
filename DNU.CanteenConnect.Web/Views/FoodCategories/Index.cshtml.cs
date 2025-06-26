// DNU.CanteenConnect.Web/Pages/FoodCategories/Index.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodCategories
{
    // Yêu cầu vai trò Admin hoặc CanteenStaff để truy cập trang này
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa danh sách các danh mục món ăn
        public IList<FoodCategory> FoodCategories { get;set; } = default!;

        // Phương thức này được gọi khi trang được yêu cầu (GET request)
        public async Task OnGetAsync()
        {
            // Lấy tất cả các danh mục món ăn từ database và gán vào thuộc tính FoodCategories
            FoodCategories = await _context.FoodCategories.ToListAsync();
        }
    }
}