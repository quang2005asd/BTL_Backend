// DNU.CanteenConnect.Web/Models/Canteen.cs
using System.ComponentModel.DataAnnotations; // Thêm using này

namespace DNU.CanteenConnect.Web.Models
{
    public class Canteen
    {
        [Key] // Đánh dấu CanteenId là khóa chính
        public int CanteenId { get; set; }

        [Required(ErrorMessage = "Tên nhà ăn là bắt buộc.")] // Tên là bắt buộc
        [StringLength(100, ErrorMessage = "Tên nhà ăn không được vượt quá 100 ký tự.")] // Giới hạn độ dài
        [Display(Name = "Tên Nhà ăn")] // Tên hiển thị trong UI
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Vị trí nhà ăn là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vị trí không được vượt quá 255 ký tự.")]
        [Display(Name = "Vị trí")]
        public string Location { get; set; } = default!;

        [Required(ErrorMessage = "Giờ mở cửa là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Giờ mở cửa không được vượt quá 100 ký tự.")]
        [Display(Name = "Giờ mở cửa")]
        public string OpeningHours { get; set; } = default!;

        // Navigation properties (nếu có)
        public ICollection<DailyMenu>? DailyMenus { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public IEnumerable<FoodItem>? FoodItems { get; set; }
    }
}