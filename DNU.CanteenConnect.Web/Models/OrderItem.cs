// DNU.CanteenConnect.Web/Models/OrderItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [ForeignKey("Order")] // Khóa ngoại tới bảng Order
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!; // Navigation property

        [ForeignKey("FoodItem")] // Khóa ngoại tới bảng FoodItem
        public int ItemId { get; set; } // Giữ ItemId để khớp với MenuItem và FoodItem
        public FoodItem FoodItem { get; set; } = default!; // Navigation property

        [Required]
        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; } // Số lượng của món ăn này trong đơn hàng

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá tại thời điểm đặt")]
        public decimal PriceAtOrder { get; set; } // Lưu lại giá của món ăn tại thời điểm đặt hàng (để tránh thay đổi giá sau này)
    }
}