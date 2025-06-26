using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    public class FoodItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FoodItems
                                               .Include(f => f.FoodCategory)
                                               .Include(f => f.Canteen)
                                               .OrderBy(f => f.Name);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // --- ĐÂY LÀ PHẦN DUY NHẤT ĐƯỢC THAY ĐỔI ---
            var foodItem = await _context.FoodItems
                                         .Include(f => f.FoodCategory)
                                         .Include(f => f.Canteen)
                                         .Include(f => f.Reviews) // Tải kèm danh sách các đánh giá
                                             .ThenInclude(r => r.User) // Từ đánh giá, tải kèm thông tin người dùng
                                         .FirstOrDefaultAsync(m => m.ItemId == id);
            // ------------------------------------------

            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItem);
        }

        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Create()
        {
            ViewData["FoodCategoryCategoryId"] = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name");
            ViewData["CanteenId"] = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Create([Bind("ItemId,Name,Description,Price,ImageUrl,IsAvailable,IsSpecialOfTheDay,FoodCategoryCategoryId,CanteenId")] FoodItem foodItem)
        {
            ModelState.Remove("FoodCategory");
            ModelState.Remove("Canteen");

            if (ModelState.IsValid)
            {
                _context.Add(foodItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodCategoryCategoryId"] = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name", foodItem.FoodCategoryCategoryId);
            ViewData["CanteenId"] = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name", foodItem.CanteenId);
            return View(foodItem);
        }

        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }
            ViewData["FoodCategoryCategoryId"] = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name", foodItem.FoodCategoryCategoryId);
            ViewData["CanteenId"] = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name", foodItem.CanteenId);
            return View(foodItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Name,Description,Price,ImageUrl,IsAvailable,IsSpecialOfTheDay,FoodCategoryCategoryId,CanteenId")] FoodItem foodItem)
        {
            if (id != foodItem.ItemId)
            {
                return NotFound();
            }
            
            ModelState.Remove("FoodCategory");
            ModelState.Remove("Canteen");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodItemExists(foodItem.ItemId))
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
            ViewData["FoodCategoryCategoryId"] = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name", foodItem.FoodCategoryCategoryId);
            ViewData["CanteenId"] = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name", foodItem.CanteenId);
            return View(foodItem);
        }
        
        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodItem = await _context.FoodItems
                                         .Include(f => f.FoodCategory)
                                         .Include(f => f.Canteen)
                                         .FirstOrDefaultAsync(m => m.ItemId == id);
            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem != null)
            {
                _context.FoodItems.Remove(foodItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FoodItemExists(int id)
        {
            return _context.FoodItems.Any(e => e.ItemId == id);
        }
    }
}