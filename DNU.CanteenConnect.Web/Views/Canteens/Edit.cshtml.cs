// DNU.CanteenConnect.Web/Pages/Canteens/Edit.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Thêm using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm using này
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.Canteens
{
    // Yêu cầu vai trò Admin hoặc CanteenStaff để truy cập trang này
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để chứa đối tượng Canteen được chỉnh sửa
        [BindProperty]
        public Canteen Canteen { get; set; } = default!;

        // Phương thức được gọi khi trang được yêu cầu với ID (GET request)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm nhà ăn trong database bằng ID
            var canteen = await _context.Canteens.FirstOrDefaultAsync(m => m.CanteenId == id);
            if (canteen == null)
            {
                return NotFound();
            }
            Canteen = canteen;
            return Page();
        }

        // Phương thức được gọi khi form được gửi đi (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra xem dữ liệu từ form có hợp lệ không
            if (!ModelState.IsValid)
            {
                return Page(); // Hiển thị lại form với lỗi nếu không hợp lệ
            }

            // Cập nhật trạng thái của đối tượng Canteen trong DbContext thành Modified
            _context.Attach(Canteen).State = EntityState.Modified;

            try
            {
                // Lưu các thay đổi vào database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý lỗi đồng thời: nếu nhà ăn đã bị xóa hoặc thay đổi bởi người khác
                if (!CanteenExists(Canteen.CanteenId))
                {
                    return NotFound(); // Không tìm thấy nhà ăn
                }
                else
                {
                    throw; // Ném lại lỗi nếu không phải là lỗi không tìm thấy
                }
            }

            // Chuyển hướng về trang Index (danh sách nhà ăn) sau khi lưu thành công
            return RedirectToPage("./Index");
        }

        // Phương thức kiểm tra xem nhà ăn có tồn tại không
        private bool CanteenExists(int id)
        {
            return _context.Canteens.Any(e => e.CanteenId == id);
        }
    }
}
