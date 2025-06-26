// Path: DNU.CanteenConnect.Web/Controllers/CanteensController.cs
using DNU.CanteenConnect.Web.Data; // Để sử dụng ApplicationDbContext
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Để sử dụng các extension methods của EF Core
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    // Controller này sẽ xử lý các yêu cầu cho Canteens (dành cho Views)
    [Authorize(Roles = "Admin,CanteenStaff")] // Áp dụng Authorization cho toàn bộ Controller
    public class CanteensController : Controller
    {
        private readonly ApplicationDbContext _context; // Quay lại sử dụng ApplicationDbContext trực tiếp

        public CanteensController(ApplicationDbContext context) // Inject ApplicationDbContext
        {
            _context = context;
        }

        // GET: Canteens (Thay thế Index.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách Canteen trực tiếp từ DbContext và sắp xếp theo tên
            return View(await _context.Canteens.OrderBy(c => c.Name).ToListAsync());
        }

        // GET: Canteens/Details/5 (Thay thế Details.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy Canteen theo ID trực tiếp từ DbContext
            var canteen = await _context.Canteens.FirstOrDefaultAsync(m => m.CanteenId == id);
            if (canteen == null)
            {
                return NotFound();
            }

            return View(canteen);
        }

        // GET: Canteens/Create (Thay thế Create.cshtml.cs OnGet)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Canteens/Create (Thay thế Create.cshtml.cs OnPostAsync)
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF
        public async Task<IActionResult> Create([Bind("CanteenId,Name,Location,OpeningHours")] Canteen canteen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(canteen); // Thêm Canteen trực tiếp vào DbContext
                await _context.SaveChangesAsync(); // Lưu thay đổi
                TempData["SuccessMessage"] = "Nhà ăn đã được thêm thành công!"; // Giữ lại thông báo
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang Index
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm nhà ăn. Vui lòng kiểm tra lại thông tin."; // Giữ lại thông báo
            return View(canteen);
        }

        // GET: Canteens/Edit/5 (Thay thế Edit.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy Canteen theo ID trực tiếp từ DbContext
            var canteen = await _context.Canteens.FindAsync(id);
            if (canteen == null)
            {
                return NotFound();
            }
            return View(canteen);
        }

        // POST: Canteens/Edit/5 (Thay thế Edit.cshtml.cs OnPostAsync)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CanteenId,Name,Location,OpeningHours")] Canteen canteen)
        {
            if (id != canteen.CanteenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(canteen); // Cập nhật Canteen trực tiếp trong DbContext
                    await _context.SaveChangesAsync(); // Lưu thay đổi
                    TempData["SuccessMessage"] = "Nhà ăn đã được cập nhật thành công!"; // Giữ lại thông báo
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CanteenExists(canteen.CanteenId)) // Sử dụng hàm kiểm tra tồn tại của Controller
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật nhà ăn. Vui lòng kiểm tra lại thông tin."; // Giữ lại thông báo
            return View(canteen);
        }

        // GET: Canteens/Delete/5 (Thay thế Delete.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy Canteen theo ID trực tiếp từ DbContext
            var canteen = await _context.Canteens.FirstOrDefaultAsync(m => m.CanteenId == id);
            if (canteen == null)
            {
                return NotFound();
            }

            return View(canteen);
        }

        // POST: Canteens/Delete/5 (Thay thế Delete.cshtml.cs OnPostAsync)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var canteen = await _context.Canteens.FindAsync(id);
            if (canteen != null)
            {
                _context.Canteens.Remove(canteen); // Xóa Canteen trực tiếp trong DbContext
                await _context.SaveChangesAsync(); // Lưu thay đổi
                TempData["SuccessMessage"] = "Nhà ăn đã được xóa thành công!"; // Giữ lại thông báo
            } else {
                TempData["ErrorMessage"] = "Không tìm thấy nhà ăn để xóa."; // Thêm thông báo nếu không tìm thấy
            }
            return RedirectToAction(nameof(Index));
        }

        // Hàm kiểm tra Canteen tồn tại (đã được khôi phục về trạng thái cũ)
        private bool CanteenExists(int id)
        {
            return _context.Canteens.Any(e => e.CanteenId == id);
        }
    }
}
