// DNU.CanteenConnect.Web/Pages/Canteens/Index.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này

namespace DNU.CanteenConnect.Web.Pages.Canteens
{
    // Yêu cầu người dùng đã đăng nhập để truy cập trang này
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa danh sách các nhà ăn
        public IList<Canteen> Canteens { get;set; } = default!;

        // Phương thức này được gọi khi trang được yêu cầu (GET request)
        public async Task OnGetAsync()
        {
            // Lấy tất cả các nhà ăn từ database và gán vào thuộc tính Canteens
            Canteens = await _context.Canteens.ToListAsync();
        }
    }
}