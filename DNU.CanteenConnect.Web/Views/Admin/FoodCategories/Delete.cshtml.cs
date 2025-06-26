using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;

namespace DNU.CanteenConnect.Web.Pages.Admin.FoodCategories
{
    public class DeleteModel : PageModel
    {
        private readonly DNU.CanteenConnect.Web.Data.ApplicationDbContext _context;

        public DeleteModel(DNU.CanteenConnect.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public FoodCategory FoodCategory { get; set; } = default!;

        // Phương thức được gọi khi trang Delete được yêu cầu (GET) với một ID
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FoodCategories == null)
            {
                return NotFound();
            }

            // Tìm danh mục món ăn theo ID
            var foodcategory = await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);

            if (foodcategory == null)
            {
                return NotFound();
            }
            else 
            {
                FoodCategory = foodcategory; // Gán danh mục tìm được để hiển thị thông tin xác nhận xóa
            }
            return Page();
        }

        // Phương thức được gọi khi form xác nhận xóa được gửi (POST)
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.FoodCategories == null)
            {
                return NotFound();
            }
            // Tìm danh mục theo ID để xóa
            var foodcategory = await _context.FoodCategories.FindAsync(id);

            if (foodcategory != null)
            {
                FoodCategory = foodcategory;
                _context.FoodCategories.Remove(FoodCategory); // Xóa danh mục khỏi DbSet
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            return RedirectToPage("./Index"); // Chuyển hướng về trang Index sau khi xóa thành công
        }
    }
}