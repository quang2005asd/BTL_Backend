// DNU.CanteenConnect.Web/Controllers/FoodCategoriesController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For Entity Framework Core methods
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    // Controller này sẽ xử lý các yêu cầu cho FoodCategories
    [Authorize(Roles = "Admin,CanteenStaff")] // Áp dụng Authorization cho toàn bộ Controller
    public class FoodCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FoodCategories (Thay thế Index.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Index()
        {
            return View(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync());
        }

        // GET: FoodCategories/Details/5 (Thay thế Details.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            return View(foodCategory);
        }

        // GET: FoodCategories/Create (Thay thế Create.cshtml.cs OnGet)
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodCategories/Create (Thay thế Create.cshtml.cs OnPostAsync)
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Description")] FoodCategory foodCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foodCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang Index
            }
            return View(foodCategory);
        }

        // GET: FoodCategories/Edit/5 (Thay thế Edit.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategories.FindAsync(id);
            if (foodCategory == null)
            {
                return NotFound();
            }
            return View(foodCategory);
        }

        // POST: FoodCategories/Edit/5 (Thay thế Edit.cshtml.cs OnPostAsync)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description")] FoodCategory foodCategory)
        {
            if (id != foodCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodCategoryExists(foodCategory.CategoryId))
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
            return View(foodCategory);
        }

        // GET: FoodCategories/Delete/5 (Thay thế Delete.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            return View(foodCategory);
        }

        // POST: FoodCategories/Delete/5 (Thay thế Delete.cshtml.cs OnPostAsync)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodCategory = await _context.FoodCategories.FindAsync(id);
            if (foodCategory != null)
            {
                _context.FoodCategories.Remove(foodCategory);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FoodCategoryExists(int id)
        {
            return _context.FoodCategories.Any(e => e.CategoryId == id);
        }
    }
}
