// DNU.CanteenConnect.Web/Pages/FoodItems/Index.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodItems
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

        // Thuộc tính để chứa danh sách các món ăn
        public IList<FoodItem> FoodItems { get;set; } = default!;

        // Phương thức này được gọi khi trang được yêu cầu (GET request)
        public async Task OnGetAsync()
        {
            // Lấy tất cả các món ăn từ database, bao gồm cả thông tin FoodCategory liên quan
            // .Include(f => f.FoodCategory) để tải dữ liệu danh mục cùng lúc với món ăn.
            FoodItems = await _context.FoodItems
                .Include(f => f.FoodCategory)
                .ToListAsync();
        }
    }
}