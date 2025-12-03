using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string UserId { get; set; } // Reference to User (Customer)

        public int? OrderId { get; set; } // Reference to Order (optional - can be null for general notifications)

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } // Notification message

        [Required]
        public string NotificationType { get; set; } // "OrderStatusUpdate", "PaymentConfirmed", "General", etc.

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public bool IsRead { get; set; } = false; // Track if user has read the notification

        // Foreign Keys
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
