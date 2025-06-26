// DNU.CanteenConnect.Web/Pages/FoodCategories/Create.cshtml.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Pages.FoodCategories
{
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FoodCategory FoodCategory { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // IMPORTANT: Khi sử dụng ValueGeneratedNever cho CategoryId trong OnModelCreating,
            // bạn phải gán một ID thủ công cho FoodCategory mới HOẶC
            // thay đổi cấu hình để nó tự động tăng.
            // Cách tốt nhất là để database tự động tăng ID khi thêm mới,
            // trong khi vẫn duy trì ID cố định cho seed data.

            // Để giải quyết lỗi "Cannot insert the value NULL into column 'CategoryId'",
            // chúng ta cần đảm bảo CategoryId được gán một giá trị hợp lệ.
            // Nếu bạn muốn ID tự động tăng cho các mục mới (nhưng vẫn giữ ID cố định cho seed data),
            // bạn cần thay đổi cấu hình trong ApplicationDbContext.cs
            // để chỉ rõ Identity cho CategoryId.

            // Tuy nhiên, để tương thích với seed data đã có ID cố định,
            // chúng ta sẽ cần đảm bảo rằng FoodCategory.CategoryId không được thiết lập là ValueGeneratedNever
            // cho các bản ghi được thêm từ UI.

            // Dựa trên lỗi, có vẻ như bạn đang cố gắng thêm một Category
            // mà không gán CategoryId, và database mong đợi nó.

            // Một cách tiếp cận phổ biến là để EF Core tự động quản lý ID cho các bản ghi mới.
            // Nếu bạn đã seed data với ID cố định, chúng ta sẽ cần đảm bảo rằng
            // ID tự động tăng sẽ không trùng với ID đã seed.

            // Để xử lý lỗi hiện tại mà không thay đổi toàn bộ chiến lược ID cho CategoryId,
            // bạn có thể thêm logic để tìm CategoryId lớn nhất hiện có và tăng nó lên.
            // Tuy nhiên, cách này không tối ưu và có thể gây lỗi trùng ID nếu có nhiều người dùng thêm đồng thời.

            // Giải pháp tối ưu:
            // 1. Thay đổi cấu hình trong ApplicationDbContext.cs để CategoryId là Identity.
            // 2. Nếu đã có seed data với ID cố định, hãy đảm bảo rằng ID tự động tăng
            //    sẽ bắt đầu từ một số lớn hơn ID lớn nhất đã seed.

            // TẠM THỜI: Để tiếp tục, chúng ta sẽ cho phép EF Core tự động quản lý Id
            // và đảm bảo rằng FoodCategory.CategoryId không bị đặt thành ValueGeneratedNever
            // trong ApplicationDbContext cho trường hợp này.

            // Vì lỗi là 'Cannot insert NULL into CategoryId', chúng ta cần đảm bảo rằng
            // EF Core biết cách gán một ID khi thêm mới.
            // Điều này thường có nghĩa là bạn nên thiết lập CategoryId trong model FoodCategory
            // và trong migration để tự động tăng (IDENTITY).
            // Nếu bạn đã cố gắng đặt ValueGeneratedNever trong DbContext, hãy bỏ nó đi cho FoodCategory.

            // Quan trọng: Hãy đảm bảo rằng trong FoodCategory.cs, CategoryId KHÔNG CÓ
            // [DatabaseGenerated(DatabaseGeneratedOption.None)] nếu bạn muốn nó tự động tăng.
            // Và trong ApplicationDbContext.cs, hãy bỏ dòng
            // modelBuilder.Entity<FoodCategory>().Property(fc => fc.CategoryId).ValueGeneratedNever(); (nếu có)
            // hoặc thêm modelBuilder.Entity<FoodCategory>().Property(fc => fc.CategoryId).UseIdentityColumn();

            // Với thiết lập hiện tại (CategoryId trong model là int, không có Identity trong migration),
            // lỗi này cho thấy EF Core không biết cách tạo ID mới.

            // Chúng ta sẽ cần chỉnh sửa lại ApplicationDbContext.cs
            // để khai báo rõ ràng rằng CategoryId là tự động tăng (IDENTITY).

            // Bước khắc phục chi tiết đã được cung cấp trong phản hồi tiếp theo.
            // Sau khi chỉnh sửa ApplicationDbContext.cs và tạo lại migration,
            // code này sẽ hoạt động mà không cần gán ID thủ công.

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.FoodCategories.Add(FoodCategory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
