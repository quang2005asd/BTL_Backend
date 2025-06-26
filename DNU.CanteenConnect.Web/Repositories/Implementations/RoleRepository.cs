// Path: DNU.CanteenConnect.Web/Repositories/RoleRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai RoleRepository
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Hiện tại, không có phương thức đặc thù nào được định nghĩa trong IRoleRepository.
        // Tương tự User, hầu hết các thao tác với Role (ví dụ: tạo, xóa, gán vai trò)
        // sẽ được xử lý bởi RoleManager của ASP.NET Core Identity.
        // Repository này có thể được sử dụng cho các truy vấn dữ liệu tùy chỉnh
        // trên bảng Role mà RoleManager không cung cấp trực tiếp.
    }
}