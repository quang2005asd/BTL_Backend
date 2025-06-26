using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DNU.CanteenConnect.Web.Models
{
    // Lớp này dùng để đại diện cho một vai trò có thể được chọn
    public class RoleSelection
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    // ViewModel chính cho trang
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleSelection> Roles { get; set; }

        public ManageUserRolesViewModel()
        {
            Roles = new List<RoleSelection>();
        }
    }
}