// DNU.CanteenConnect.Web/Models/Order.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày Đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Mặc định là Chờ xử lý

        [StringLength(1000)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Nhà ăn là bắt buộc.")]
        [Display(Name = "Nhà ăn")]
        public int CanteenId { get; set; }

        [ForeignKey("CanteenId")]
        public Canteen? Canteen { get; set; }

        // ĐÃ THÊM: Thuộc tính để lưu phương thức thanh toán
        [Required]
        [StringLength(50)]
        [Display(Name = "Phương thức Thanh toán")]
        public string PaymentMethod { get; set; } = "CashOnDelivery"; // Mặc định là Thanh toán khi nhận hàng


        // Navigation property
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}