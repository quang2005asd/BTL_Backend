// DNU.CanteenConnect.Web/Pages/DailyMenus/Create.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace DNU.CanteenConnect.Web.Pages.DailyMenus
{
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public DailyMenu DailyMenu { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Món ăn trong thực đơn")]
        public List<int> SelectedFoodItemIds { get; set; } = new List<int>();

        public SelectList CanteenOptions { get; set; } = default!;
        public MultiSelectList FoodItemOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            CanteenOptions = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name");
            
            // Ban đầu, FoodItemOptions sẽ trống hoặc chứa tất cả các món ăn có sẵn (nếu không có CanteenId được chọn)
            // Nó sẽ được điền lại bằng AJAX khi người dùng chọn Nhà ăn.
            FoodItemOptions = new MultiSelectList(new List<FoodItem>(), "ItemId", "Name"); 

            DailyMenu = new DailyMenu { MenuDate = System.DateTime.Today };

            return Page();
        }

        // Phương thức Handler mới để trả về danh sách món ăn dựa trên CanteenId
        public async Task<JsonResult> OnGetFoodItemsByCanteenAsync(int canteenId)
        {
            var foodItems = await _context.FoodItems
                                          .Where(f => f.IsAvailable && f.CanteenId == canteenId) // Lọc theo CanteenId
                                          .OrderBy(f => f.Name)
                                          .Select(f => new SelectListItem
                                          {
                                              Value = f.ItemId.ToString(),
                                              Text = f.Name
                                          })
                                          .ToListAsync();
            
            return new JsonResult(foodItems);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Cần loại bỏ FoodItemOptions khỏi ModelState nếu không nó sẽ gây lỗi validation
            // vì nó không phải là một input thực sự từ form
            ModelState.Remove("FoodItemOptions");

            if (!ModelState.IsValid)
            {
                CanteenOptions = new SelectList(await _context.Canteens.OrderBy(c => c.Name).ToListAsync(), "CanteenId", "Name");
                
                // Khi có lỗi validation, chúng ta cần điền lại FoodItemOptions dựa trên CanteenId đã chọn
                var selectedCanteenId = DailyMenu.CanteenId;
                if (selectedCanteenId.HasValue)
                {
                    var availableFoodItems = await _context.FoodItems
                                                           .Where(f => f.IsAvailable && f.CanteenId == selectedCanteenId.Value)
                                                           .OrderBy(f => f.Name)
                                                           .ToListAsync();
                    FoodItemOptions = new MultiSelectList(availableFoodItems, "ItemId", "Name", SelectedFoodItemIds);
                }
                else
                {
                    FoodItemOptions = new MultiSelectList(new List<FoodItem>(), "ItemId", "Name");
                }
                return Page();
            }

            _context.DailyMenus.Add(DailyMenu);
            await _context.SaveChangesAsync();

            foreach (var foodItemId in SelectedFoodItemIds)
            {
                var menuItem = new MenuItem
                {
                    MenuId = DailyMenu.MenuId,
                    ItemId = foodItemId
                };
                _context.MenuItems.Add(menuItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
