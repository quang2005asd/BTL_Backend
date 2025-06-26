using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }

        [ForeignKey("DailyMenu")] // Khóa ngoại tới bảng DailyMenu
        public int MenuId { get; set; }
        public DailyMenu DailyMenu { get; set; } = default!; // Navigation property

        [ForeignKey("FoodItem")] // Khóa ngoại tới bảng FoodItem
        public int ItemId { get; set; }
        public FoodItem FoodItem { get; set; } = default!; // Navigation property

        public int? QuantityAvailable { get; set; } // Số lượng giới hạn cho món này trong thực đơn ngày đó, có thể null (không giới hạn)
    }
}