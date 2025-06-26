using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            // Lấy các món ăn, lấy kèm review, tính toán và đóng gói vào ViewModel
            viewModel.FeaturedFoodItems = await _context.FoodItems
                                                        // --- THAY ĐỔI DUY NHẤT NẰM Ở ĐÂY ---
                                                        // Lọc những món vừa là "Đặc biệt", vừa "Có sẵn"
                                                        .Where(f => f.IsSpecialOfTheDay == true && f.IsAvailable == true) 
                                                        .Include(f => f.Reviews) // Lấy kèm review để tính toán
                                                        .Select(f => new MenuItemViewModel
                                                        {
                                                            FoodItem = f,
                                                            AverageRating = f.Reviews.Any() ? f.Reviews.Average(r => r.Rating) : 0,
                                                            ReviewCount = f.Reviews.Count()
                                                        })
                                                        .ToListAsync(); // Bỏ Take(6) để hiển thị tất cả món đặc biệt
            
            return View(viewModel);
        }
        
        // --- CÁC PHẦN SAU ĐƯỢC GIỮ NGUYÊN TỪ CODE CỦA BẠN ---
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