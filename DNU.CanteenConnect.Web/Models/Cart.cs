// DNU.CanteenConnect.Web/Models/Cart.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        // Foreign key to User
        // Each cart belongs to a specific user
        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey("UserId")]
        public User User { get; set; } = default!;

        // Cart creation time
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Last updated time
        [Display(Name = "Ngày cập nhật")]
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        // Navigation property for items in the cart
        public ICollection<CartItem>? CartItems { get; set; }
    }
}