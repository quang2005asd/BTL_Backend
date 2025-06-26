// DNU.CanteenConnect.Web/Models/User.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System; // Thêm using này để sử dụng DateTime

namespace DNU.CanteenConnect.Web.Models
{
    // Kế thừa từ IdentityUser để mở rộng các thuộc tính người dùng
    public class User : IdentityUser
    {
        // Các thuộc tính của IdentityUser bao gồm:
        // Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        // PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
        // TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount

        // Bạn có thể thêm các thuộc tính tùy chỉnh của mình ở đây.
        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; } // Cho phép null nếu địa chỉ không bắt buộc

        [Required] // Yêu cầu trường này phải có giá trị
        [DataType(DataType.DateTime)] // Chỉ định kiểu dữ liệu là DateTime
        [Display(Name = "Ngày tham gia")] // Tên hiển thị trong UI
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Mặc định là thời gian hiện tại khi tạo
    }
}