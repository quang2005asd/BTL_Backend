using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;

namespace DNU.CanteenConnect.Web.Pages.Admin.FoodCategories
{
    public class CreateModel : PageModel
    {
        private readonly DNU.CanteenConnect.Web.Data.ApplicationDbContext _context;

        public CreateModel(DNU.CanteenConnect.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Phương thức được gọi khi trang được yêu cầu (GET)
        public IActionResult OnGet()
        {
            return Page();
        }

        // Property để ràng buộc dữ liệu từ form
        [BindProperty] // Đảm bảo model này được ràng buộc khi gửi form (POST)
        public FoodCategory FoodCategory { get; set; } = default!;

        // Phương thức được gọi khi form được gửi (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra xem dữ liệu gửi lên có hợp lệ theo các Data Annotations trong Models không
            // Đồng thời kiểm tra xem FoodCategory có null không
            if (!ModelState.IsValid || _context.FoodCategories == null || FoodCategory == null)
            {
                // Nếu không hợp lệ, quay lại trang và hiển thị lỗi
                return Page();
            }

            // Thêm đối tượng FoodCategory mới vào DbSet
            _context.FoodCategories.Add(FoodCategory);
            // Lưu các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Chuyển hướng về trang Index sau khi thêm thành công
            return RedirectToPage("./Index");
        }
    }
}