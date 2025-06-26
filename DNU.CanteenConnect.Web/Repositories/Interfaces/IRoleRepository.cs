// Path: DNU.CanteenConnect.Web/Interfaces/IRoleRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho RoleRepository, kế thừa từ IRepository chung
    public interface IRoleRepository : IRepository<Role>
    {
        // Có thể thêm các phương thức đặc thù cho Role nếu cần,
        // ví dụ: GetRoleWithUsersAsync, GetRolesByUserId, v.v.
        // Tương tự User, các thao tác quản lý vai trò thường qua RoleManager.
    }
}