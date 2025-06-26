// Path: DNU.CanteenConnect.Web/Repositories/UserRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai UserRepository
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Hiện tại, không có phương thức đặc thù nào được định nghĩa trong IUserRepository.
        // Hầu hết các thao tác với User (ví dụ: đăng ký, đăng nhập, quản lý vai trò)
        // sẽ được xử lý bởi UserManager của ASP.NET Core Identity.
        // Repository này có thể được sử dụng cho các truy vấn dữ liệu tùy chỉnh
        // trên bảng User mà UserManager không cung cấp trực tiếp.
    }
}