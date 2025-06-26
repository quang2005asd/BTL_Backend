using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Menu hoặc /Menu/Index
        public async Task<IActionResult> Index(DateTime? menuDate, int? canteenId)
        {
            // GIỮ NGUYÊN PHẦN CHUẨN BỊ CỦA BẠN
            DateTime dateToFilter = menuDate ?? DateTime.Today;
            ViewBag.CanteenOptions = new SelectList(await _context.Canteens.ToListAsync(), "CanteenId", "Name", canteenId);
            ViewBag.SelectedDate = dateToFilter;

            // GIỮ NGUYÊN CÁCH TRUY VẤN BAN ĐẦU CỦA BẠN
            var query = _context.DailyMenus
                                .Include(dm => dm.MenuItems)!
                                    .ThenInclude(mi => mi.FoodItem)!
                                        .ThenInclude(fi => fi.FoodCategory)
                                .Include(dm => dm.MenuItems)!
                                    .ThenInclude(mi => mi.FoodItem)!
                                        .ThenInclude(fi => fi.Canteen) // Thêm Canteen vào đây
                                .Where(dm => dm.MenuDate.Date == dateToFilter.Date);

            if (canteenId.HasValue && canteenId.Value > 0)
            {
                query = query.Where(dm => dm.CanteenId == canteenId.Value);
            }

            var dailyMenus = await query.ToListAsync();

            // GIỮ NGUYÊN CÁC VÒNG LẶP FOREACH CỦA BẠN
            var displayedFoodItems = new List<FoodItem>();
            foreach (var dailyMenu in dailyMenus)
            {
                if (dailyMenu.MenuItems != null)
                {
                    foreach (var menuItem in dailyMenu.MenuItems)
                    {
                        if (menuItem.FoodItem != null && menuItem.FoodItem.IsAvailable)
                        {
                            displayedFoodItems.Add(menuItem.FoodItem);
                        }
                    }
                }
            }
            
            var distinctFoodItems = displayedFoodItems.DistinctBy(fi => fi.ItemId).ToList();

            // PHẦN BỔ SUNG ĐỂ LẤY ĐÁNH GIÁ MÀ KHÔNG THAY ĐỔI LOGIC GỐC
            var menuToDisplay = new List<MenuItemViewModel>();
            foreach (var foodItem in distinctFoodItems)
            {
                // Với mỗi món ăn, hỏi lại CSDL để lấy riêng review của nó
                var reviews = await _context.Reviews
                                            .Where(r => r.ItemId == foodItem.ItemId)
                                            .ToListAsync();

                menuToDisplay.Add(new MenuItemViewModel
                {
                    FoodItem = foodItem,
                    AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                    ReviewCount = reviews.Count
                });
            }
            // KẾT THÚC PHẦN BỔ SUNG

            return View(menuToDisplay);
        }

        // CẬP NHẬT LẠI ACTION NÀY THEO LOGIC GỐC CỦA BẠN
        [HttpGet]
        public async Task<IActionResult> DailyMenuItems(DateTime? menuDate, int? canteenId)
        {
            DateTime dateToFilter = menuDate ?? DateTime.Today;

            var query = _context.DailyMenus
                                .Include(dm => dm.MenuItems)!
                                    .ThenInclude(mi => mi.FoodItem)!
                                        .ThenInclude(fi => fi.FoodCategory)
                                .Include(dm => dm.MenuItems)!
                                    .ThenInclude(mi => mi.FoodItem)!
                                        .ThenInclude(fi => fi.Canteen)
                                .Where(dm => dm.MenuDate.Date == dateToFilter.Date);

            if (canteenId.HasValue && canteenId.Value > 0)
            {
                query = query.Where(dm => dm.CanteenId == canteenId.Value);
            }

            var dailyMenus = await query.ToListAsync();

            var displayedFoodItems = new List<FoodItem>();
            foreach (var dailyMenu in dailyMenus)
            {
                if (dailyMenu.MenuItems != null)
                {
                    foreach (var menuItem in dailyMenu.MenuItems)
                    {
                        if (menuItem.FoodItem != null && menuItem.FoodItem.IsAvailable)
                        {
                            displayedFoodItems.Add(menuItem.FoodItem);
                        }
                    }
                }
            }
            
            var distinctFoodItems = displayedFoodItems.DistinctBy(fi => fi.ItemId).ToList();

            // PHẦN BỔ SUNG ĐỂ LẤY ĐÁNH GIÁ
            var resultsWithReviews = new List<object>();
            foreach (var foodItem in distinctFoodItems)
            {
                var reviews = await _context.Reviews
                                            .Where(r => r.ItemId == foodItem.ItemId)
                                            .ToListAsync();
                
                resultsWithReviews.Add(new
                {
                    itemId = foodItem.ItemId,
                    name = foodItem.Name,
                    description = foodItem.Description,
                    price = foodItem.Price,
                    imageUrl = foodItem.ImageUrl,
                    canteenName = foodItem.Canteen?.Name,
                    categoryName = foodItem.FoodCategory?.Name,
                    averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                    reviewCount = reviews.Count
                });
            }

            return Json(resultsWithReviews);
        }
    }
}