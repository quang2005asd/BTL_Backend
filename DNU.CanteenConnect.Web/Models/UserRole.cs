using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần thiết cho [ForeignKey]

namespace DNU.CanteenConnect.Web.Models
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [ForeignKey("User")] // Khóa ngoại tới bảng User
        public int UserId { get; set; }
        public User User { get; set; } = default!; // Navigation property

        [ForeignKey("Role")] // Khóa ngoại tới bảng Role
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!; // Navigation property
    }
}