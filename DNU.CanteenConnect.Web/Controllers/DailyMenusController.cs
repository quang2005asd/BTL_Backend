// DNU.CanteenConnect.Web/Controllers/DailyMenusController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize(Roles = "Admin,CanteenStaff")] // Apply Authorization for the entire Controller
    public class DailyMenusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyMenusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyMenus (Replaces Index.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DailyMenus.Include(d => d.Canteen).OrderByDescending(d => d.MenuDate);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DailyMenus/Details/5 (Replaces Details.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyMenu = await _context.DailyMenus
                .Include(d => d.Canteen)
                .Include(d => d.MenuItems)! // Include MenuItems
                .ThenInclude(mi => mi.FoodItem)! // Then include FoodItem for each MenuItem
                .ThenInclude(fi => fi.FoodCategory) // Then include FoodCategory for each FoodItem
                .FirstOrDefaultAsync(m => m.MenuId == id);

            if (dailyMenu == null)
            {
                return NotFound();
            }

            return View(dailyMenu);
        }

        // GET: DailyMenus/Create (Replaces Create.cshtml.cs OnGet)
        public IActionResult Create()
        {
            // Populate ViewBag for Canteen dropdown
            ViewData["CanteenId"] = new SelectList(_context.Canteens, "CanteenId", "Name");
            return View();
        }

        // POST: DailyMenus/Create (Replaces Create.cshtml.cs OnPostAsync)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuId,MenuDate,CanteenId,Notes")] DailyMenu dailyMenu)
        {
            // Set MenuId to 0 to ensure EF Core generates a new identity value.
            // This is important if DatabaseGeneratedOption.Identity is used for MenuId.
            dailyMenu.MenuId = 0; 
            
            if (ModelState.IsValid)
            {
                _context.Add(dailyMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CanteenId"] = new SelectList(_context.Canteens, "CanteenId", "Name", dailyMenu.CanteenId);
            return View(dailyMenu);
        }

        // GET: DailyMenus/Edit/5 (Replaces Edit.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyMenu = await _context.DailyMenus
                                            .Include(d => d.MenuItems)!
                                                .ThenInclude(mi => mi.FoodItem)
                                            .FirstOrDefaultAsync(m => m.MenuId == id);

            if (dailyMenu == null)
            {
                return NotFound();
            }
            ViewData["CanteenId"] = new SelectList(_context.Canteens, "CanteenId", "Name", dailyMenu.CanteenId);
            ViewData["FoodItemOptions"] = new SelectList(_context.FoodItems.Include(fi => fi.FoodCategory), "ItemId", "Name"); // For adding items

            return View(dailyMenu);
        }

        // POST: DailyMenus/Edit/5 (Replaces Edit.cshtml.cs OnPostAsync)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuId,MenuDate,CanteenId,Notes")] DailyMenu dailyMenu)
        {
            if (id != dailyMenu.MenuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyMenuExists(dailyMenu.MenuId))
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
            ViewData["CanteenId"] = new SelectList(_context.Canteens, "CanteenId", "Name", dailyMenu.CanteenId);
            return View(dailyMenu);
        }

        // POST: DailyMenus/AddFoodItem (New action to add food items to a daily menu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFoodItem(int menuId, int foodItemId)
        {
            var dailyMenu = await _context.DailyMenus
                                            .Include(dm => dm.MenuItems)
                                            .FirstOrDefaultAsync(dm => dm.MenuId == menuId);

            if (dailyMenu == null)
            {
                return NotFound();
            }

            var foodItem = await _context.FoodItems.FindAsync(foodItemId);
            if (foodItem == null)
            {
                return NotFound();
            }

            // Check if the item already exists in the menu
            if (dailyMenu.MenuItems == null)
            {
                dailyMenu.MenuItems = new List<MenuItem>();
            }

            // Corrected to use MenuItem's ItemId and MenuId properties
            if (!dailyMenu.MenuItems.Any(mi => mi.ItemId == foodItemId && mi.MenuId == menuId))
            {
                dailyMenu.MenuItems.Add(new MenuItem { MenuId = menuId, ItemId = foodItemId });
                await _context.SaveChangesAsync();
            }
            
            // Redirect back to the Edit page to show updated items
            return RedirectToAction(nameof(Edit), new { id = menuId });
        }

        // POST: DailyMenus/RemoveFoodItem (New action to remove food items from a daily menu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFoodItem(int menuId, int foodItemId)
        {
            // Corrected to use MenuItem's ItemId and MenuId properties
            var menuItem = await _context.MenuItems
                                            .FirstOrDefaultAsync(mi => mi.MenuId == menuId && mi.ItemId == foodItemId);

            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            // Redirect back to the Edit page to show updated items
            return RedirectToAction(nameof(Edit), new { id = menuId });
        }


        // GET: DailyMenus/Delete/5 (Replaces Delete.cshtml.cs OnGetAsync)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyMenu = await _context.DailyMenus
                .Include(d => d.Canteen)
                .FirstOrDefaultAsync(m => m.MenuId == id);
            if (dailyMenu == null)
            {
                return NotFound();
            }

            return View(dailyMenu);
        }

        // POST: DailyMenus/Delete/5 (Replaces Delete.cshtml.cs OnPostAsync)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyMenu = await _context.DailyMenus.FindAsync(id);
            if (dailyMenu != null)
            {
                _context.DailyMenus.Remove(dailyMenu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DailyMenuExists(int id)
        {
            return _context.DailyMenus.Any(e => e.MenuId == id);
        }
    }
}
