using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;

namespace DNU.CanteenConnect.Web.Pages.Admin.FoodCategories
{
    public class EditModel : PageModel
    {
        private readonly DNU.CanteenConnect.Web.Data.ApplicationDbContext _context;

        public EditModel(DNU.CanteenConnect.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FoodCategory FoodCategory { get; set; } = default!;

        // Phương thức được gọi khi trang Edit được yêu cầu (GET) với một ID
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FoodCategories == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy ID hoặc DbSet
            }

            // Tìm danh mục món ăn theo ID trong cơ sở dữ liệu
            var foodcategory =  await _context.FoodCategories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (foodcategory == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy danh mục
            }
            FoodCategory = foodcategory; // Gán danh mục tìm được vào FoodCategory để hiển thị trên form
            return Page();
        }

        // Phương thức được gọi khi form được gửi (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu dữ liệu không hợp lệ, quay lại trang
            }

            // Đánh dấu trạng thái của đối tượng là Modified để EF Core biết cần cập nhật
            _context.Attach(FoodCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý lỗi trùng lặp khi cập nhật đồng thời
                if (!FoodCategoryExists(FoodCategory.CategoryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index"); // Chuyển hướng về trang Index sau khi lưu thành công
        }

        // Phương thức kiểm tra sự tồn tại của danh mục
        private bool FoodCategoryExists(int id)
        {
          return (_context.FoodCategories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}