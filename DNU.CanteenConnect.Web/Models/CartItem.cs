// DNU.CanteenConnect.Web/Models/CartItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        // Khóa ngoại tới Cart
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; } = default!; // Navigation property

        // Khóa ngoại tới FoodItem
        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; } // Tên thuộc tính khớp với FoodItem
        public FoodItem FoodItem { get; set; } = default!; // Navigation property

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        // Lưu lại giá của món ăn tại thời điểm thêm vào giỏ hàng
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá tại thời điểm thêm")]
        public decimal PriceAtAddition { get; set; }
    }
}