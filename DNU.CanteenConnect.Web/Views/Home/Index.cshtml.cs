// DNU.CanteenConnect.Web/Pages/Index.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Add this using
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Property to hold the list of food items that will be displayed on the homepage
        public IList<FoodItem> FeaturedFoodItems { get; set; } = default!;

        // Method called when the page is requested (GET request)
        public async Task OnGetAsync()
        {
            // Retrieve food items marked as "Special Of The Day" from the database
            FeaturedFoodItems = await _context.FoodItems
                .Include(f => f.FoodCategory) // Eager load the related FoodCategory data
                .Where(f => f.IsAvailable && f.IsSpecialOfTheDay) // Filter for available and special items
                .Take(6) // Optionally limit the number of items displayed
                .ToListAsync();
        }
    }
}