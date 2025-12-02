using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Helpers; // Thư viện phân trang
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

        // --- ACTION INDEX ĐÃ ĐƯỢC NÂNG CẤP VỚI TÌM KIẾM, LỌC, PHÂN TRANG ---
        [Authorize(Roles = "Admin,CanteenStaff")] // Trang quản lý này chỉ dành cho Admin/Staff
        public async Task<IActionResult> Index(string searchString, int? canteenId, int? categoryId, int? pageNumber)
        {
            var foodItemsQuery = _context.FoodItems
                                         .Include(f => f.Canteen)
                                         .Include(f => f.FoodCategory)
                                         .AsQueryable();

            // Lọc theo từ khóa tìm kiếm (tên món ăn)
            if (!string.IsNullOrEmpty(searchString))
            {
                foodItemsQuery = foodItemsQuery.Where(s => s.Name.Contains(searchString));
            }
            // Lọc theo nhà ăn
            if (canteenId.HasValue)
            {
                foodItemsQuery = foodItemsQuery.Where(x => x.CanteenId == canteenId);
            }
            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                foodItemsQuery = foodItemsQuery.Where(x => x.FoodCategoryCategoryId == categoryId);
            }

            int pageSize = 10; // Hiển thị 10 món ăn mỗi trang
            var paginatedFoodItems = await PaginatedList<FoodItem>.CreateAsync(foodItemsQuery.OrderBy(f => f.Name), pageNumber ?? 1, pageSize);

            // Tạo ViewModel và truyền ra View
            var viewModel = new FoodItemManagementViewModel
            {
                FoodItems = paginatedFoodItems,
                Canteens = new SelectList(await _context.Canteens.ToListAsync(), "CanteenId", "Name", canteenId),
                Categories = new SelectList(await _context.FoodCategories.ToListAsync(), "CategoryId", "Name", categoryId),
                SearchString = searchString,
                CanteenId = canteenId,
                CategoryId = categoryId
            };

            return View(viewModel);
        }

        // --- CÁC ACTION KHÁC ĐƯỢC GIỮ NGUYÊN NHƯ PHIÊN BẢN TỐT NHẤT CỦA BẠN ---

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var foodItem = await _context.FoodItems
                                         .Include(f => f.FoodCategory)
                                         .Include(f => f.Canteen)
                                         .Include(f => f.Reviews)!
                                             .ThenInclude(r => r.User)
                                         .FirstOrDefaultAsync(m => m.ItemId == id);
            if (foodItem == null) return NotFound();
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
        public async Task<IActionResult> Create([Bind("ItemId,Name,Description,Price,ImageUrl,IsAvailable,IsSpecialOfTheDay,StockQuantity,FoodCategoryCategoryId,CanteenId")] FoodItem foodItem)
        {
            ModelState.Remove("FoodCategory");
            ModelState.Remove("Canteen");
            if (ModelState.IsValid)
            {
                _context.Add(foodItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm món ăn mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodCategoryCategoryId"] = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name", foodItem.FoodCategoryCategoryId);
            ViewData["CanteenId"] = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name", foodItem.CanteenId);
            return View(foodItem);
        }

        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null) return NotFound();
            ViewData["FoodCategoryCategoryId"] = new SelectList(await _context.FoodCategories.OrderBy(fc => fc.Name).ToListAsync(), "CategoryId", "Name", foodItem.FoodCategoryCategoryId);
            ViewData["CanteenId"] = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name", foodItem.CanteenId);
            return View(foodItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CanteenStaff")]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Name,Description,Price,ImageUrl,IsAvailable,IsSpecialOfTheDay,FoodCategoryCategoryId,CanteenId")] FoodItem foodItem)
        {
            if (id != foodItem.ItemId) return NotFound();
            ModelState.Remove("FoodCategory");
            ModelState.Remove("Canteen");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin món ăn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodItemExists(foodItem.ItemId)) return NotFound();
                    else throw;
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
            if (id == null) return NotFound();
            var foodItem = await _context.FoodItems
                                         .Include(f => f.FoodCategory)
                                         .Include(f => f.Canteen)
                                         .FirstOrDefaultAsync(m => m.ItemId == id);
            if (foodItem == null) return NotFound();
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
                TempData["SuccessMessage"] = $"Đã xóa món ăn '{foodItem.Name}' thành công.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FoodItemExists(int id)
        {
            return _context.FoodItems.Any(e => e.ItemId == id);
        }
    }
}