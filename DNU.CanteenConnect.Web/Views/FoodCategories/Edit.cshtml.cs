// DNU.CanteenConnect.Web/Pages/FoodCategories/Edit.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization; // Add this using
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Add this using
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodCategories
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

        // Property to hold the FoodCategory object being edited
        [BindProperty]
        public FoodCategory FoodCategory { get; set; } = default!;

        // Method called when the page is requested with an ID (GET request)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the food category in the database by ID
            var foodcategory = await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (foodcategory == null)
            {
                return NotFound();
            }
            FoodCategory = foodcategory;
            return Page();
        }

        // Method called when the form is submitted (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if data from the form is valid
            if (!ModelState.IsValid)
            {
                return Page(); // Re-display the form with errors if invalid
            }

            // Update the state of the FoodCategory object in DbContext to Modified
            _context.Attach(FoodCategory).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency error: if the food category was deleted or changed by another user
                if (!FoodCategoryExists(FoodCategory.CategoryId))
                {
                    return NotFound(); // Food category not found
                }
                else
                {
                    throw; // Re-throw the error if it's not a not-found error
                }
            }

            // Redirect to the Index page (list of food categories) after successful save
            return RedirectToPage("./Index");
        }

        // Method to check if a food category exists
        private bool FoodCategoryExists(int id)
        {
            return _context.FoodCategories.Any(e => e.CategoryId == id);
        }
    }
}
