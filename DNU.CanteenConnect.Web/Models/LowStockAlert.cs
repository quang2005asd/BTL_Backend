using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class LowStockAlert
    {
        [Key]
        public int AlertId { get; set; }

        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }
        public FoodItem? FoodItem { get; set; }

        [ForeignKey("Canteen")]
        public int CanteenId { get; set; }
        public Canteen? Canteen { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        [Range(1, 20)]
        public int ThresholdStock { get; set; } = 5; // Default threshold

        [StringLength(50)]
        public string AlertStatus { get; set; } = "Active"; // Active, Resolved

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ResolvedAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
