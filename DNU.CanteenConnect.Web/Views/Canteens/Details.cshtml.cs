// DNU.CanteenConnect.Web/Pages/Canteens/Details.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.Canteens
{
    // Yêu cầu người dùng đã đăng nhập để truy cập trang này (có thể thêm vai trò sau)
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa đối tượng Canteen được hiển thị
        public Canteen Canteen { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Kiểm tra xem ID có được cung cấp không
            if (id == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không có ID
            }

            // Tìm nhà ăn trong database bằng ID
            var canteen = await _context.Canteens.FirstOrDefaultAsync(m => m.CanteenId == id);

            // Kiểm tra xem nhà ăn có tồn tại không
            if (canteen == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy nhà ăn
            }

            // Gán nhà ăn tìm được vào thuộc tính Canteen
            Canteen = canteen;
            return Page(); // Hiển thị trang
        }
    }
}