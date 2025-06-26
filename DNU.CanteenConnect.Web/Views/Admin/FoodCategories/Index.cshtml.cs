using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;

namespace DNU.CanteenConnect.Web.Pages.Admin.FoodCategories
{
    public class IndexModel : PageModel
    {
        private readonly DNU.CanteenConnect.Web.Data.ApplicationDbContext _context;

        public IndexModel(DNU.CanteenConnect.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Property để chứa danh sách các danh mục món ăn
        public IList<FoodCategory> FoodCategory { get;set; } = default!;

        // Phương thức được gọi khi trang Index được yêu cầu (GET)
        public async Task OnGetAsync()
        {
            // Kiểm tra xem _context.FoodCategories có null không trước khi truy vấn
            if (_context.FoodCategories != null)
            {
                // Lấy tất cả danh mục món ăn từ cơ sở dữ liệu và gán vào FoodCategory để hiển thị trên trang
                FoodCategory = await _context.FoodCategories.ToListAsync();
            }
            else
            {
                // Xử lý trường hợp DbContext hoặc DbSet bị null (thường không xảy ra nếu cấu hình đúng)
                FoodCategory = new List<FoodCategory>(); // Gán một danh sách trống để tránh lỗi NullReferenceException
            }
        }
    }
}