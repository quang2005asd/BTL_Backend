// DNU.CanteenConnect.Web/Pages/FoodItems/Edit.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Add this using
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Add this using for SelectList
using Microsoft.EntityFrameworkCore; // Add this using
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodItems
{
    // Requires Admin or CanteenStaff role to access this page
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Property to hold the FoodItem object being edited
        [BindProperty]
        public FoodItem FoodItem { get; set; } = default!;

        // Property to hold the SelectList for Food Categories
        public SelectList FoodCategoryOptions { get; set; } = default!;


        // Method called when the page is requested with an ID (GET request)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the food item in the database by ID, including its FoodCategory
            var fooditem = await _context.FoodItems
                                        .Include(f => f.FoodCategory)
                                        .FirstOrDefaultAsync(m => m.ItemId == id);

            if (fooditem == null)
            {
                return NotFound();
            }
            FoodItem = fooditem;

            // Populate the dropdown list with existing FoodCategories,
            // pre-selecting the current category of the food item
            FoodCategoryOptions = new SelectList(_context.FoodCategories, "CategoryId", "Name", FoodItem.FoodCategoryCategoryId);
            return Page();
        }

        // Method called when the form is submitted (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // If the model state is not valid, re-populate dropdown and return to page
            if (!ModelState.IsValid)
            {
                FoodCategoryOptions = new SelectList(_context.FoodCategories, "CategoryId", "Name", FoodItem.FoodCategoryCategoryId);
                return Page();
            }

            // Update the state of the FoodItem object in DbContext to Modified
            _context.Attach(FoodItem).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency error: if the food item was deleted or changed by another user
                if (!FoodItemExists(FoodItem.ItemId))
                {
                    return NotFound(); // Food item not found
                }
                else
                {
                    throw; // Re-throw the error if it's not a not-found error
                }
            }

            // Redirect to the Index page (list of food items) after successful save
            return RedirectToPage("./Index");
        }

        // Method to check if a food item exists
        private bool FoodItemExists(int id)
        {
            return _context.FoodItems.Any(e => e.ItemId == id);
        }
    }
}
