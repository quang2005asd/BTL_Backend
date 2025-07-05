using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Helpers; // <-- Thêm để dùng session
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            viewModel.FeaturedFoodItems = await _context.FoodItems
                .Where(f => f.IsSpecialOfTheDay && f.IsAvailable)
                .Include(f => f.Reviews)
                .Select(f => new MenuItemViewModel
                {
                    FoodItem = f,
                    AverageRating = f.Reviews.Any() ? f.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = f.Reviews.Count()
                })
                .ToListAsync();

            return View(viewModel);
        }

        //  ACTION: Thêm món vào giỏ hàng (Session)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int foodItemId)
        {
            var foodItem = await _context.FoodItems.FindAsync(foodItemId);
            if (foodItem == null || !foodItem.IsAvailable)
            {
                TempData["ErrorMessage"] = "Món ăn không tồn tại hoặc không khả dụng.";
                return RedirectToAction("Index");
            }

            // Lấy hoặc tạo giỏ hàng trong Session
            var cart = HttpContext.Session.Get<Cart>("GuestCart") ?? new Cart();

            var existingItem = cart.CartItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    FoodItemId = foodItemId,
                    Quantity = 1,
                    PriceAtAddition = foodItem.Price
                });
            }

            // Cập nhật lại Session
            HttpContext.Session.Set("GuestCart", cart);
            TempData["SuccessMessage"] = "Đã thêm món vào giỏ hàng.";

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
