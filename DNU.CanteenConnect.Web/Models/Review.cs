using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        // --- Mối quan hệ với Món ăn (FoodItem) ---
        [Required]
        [Display(Name = "Món ăn")]
        public int ItemId { get; set; }
        public virtual FoodItem? FoodItem { get; set; }

        // --- Mối quan hệ với Người dùng (User) ---
        // UserId có thể là string nếu bạn dùng Identity, hoặc int nếu bạn tự quản lý
        [Required]
        [Display(Name = "Người đánh giá")]
        public string UserId { get; set; } // Giả sử UserId là string
        public virtual User? User { get; set; }

        [Required(ErrorMessage = "Vui lòng cho điểm đánh giá.")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao.")]
        [Display(Name = "Điểm (Sao)")]
        public int Rating { get; set; } // Ví dụ: 1, 2, 3, 4, 5

        [StringLength(2000, ErrorMessage = "Bình luận quá dài.")]
        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }

        [Required]
        [Display(Name = "Ngày đánh giá")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}