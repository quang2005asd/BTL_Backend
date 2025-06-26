// DNU.CanteenConnect.Web/Pages/DailyMenus/Index.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.DailyMenus
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

        // Thuộc tính để chứa danh sách các thực đơn hàng ngày
        public IList<DailyMenu> DailyMenus { get;set; } = default!;

        // Phương thức này được gọi khi trang được yêu cầu (GET request)
        public async Task OnGetAsync()
        {
            // Lấy tất cả các thực đơn hàng ngày từ database, bao gồm cả thông tin Canteen liên quan
            // .Include(d => d.Canteen) để tải dữ liệu nhà ăn cùng lúc với thực đơn.
            DailyMenus = await _context.DailyMenus
                .Include(d => d.Canteen)
                .OrderByDescending(d => d.MenuDate) // Sắp xếp theo ngày giảm dần
                .ToListAsync();
        }
    }
}