// Path: DNU.CanteenConnect.Web/Interfaces/IUserRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho UserRepository, kế thừa từ IRepository chung
    public interface IUserRepository : IRepository<User>
    {
        // Có thể thêm các phương thức đặc thù cho User nếu cần,
        // ví dụ: GetUserWithRolesAsync, GetUsersByCanteenId, v.v.
        // Tuy nhiên, với IdentityUser, nhiều thao tác quản lý người dùng
        // sẽ được thực hiện thông qua UserManager.
    }
}