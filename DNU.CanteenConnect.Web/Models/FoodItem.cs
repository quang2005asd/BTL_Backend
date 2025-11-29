using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models
{
    public class FoodItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Tên món ăn là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên món ăn không được vượt quá 255 ký tự.")]
        [Display(Name = "Tên Món ăn")]
        public string Name { get; set; } = default!;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá món ăn là bắt buộc.")]
        [Range(0.01, 100000000.00, ErrorMessage = "Giá phải lớn hơn 0.")]
        [Column(TypeName = "decimal(18, 2)")] // Định dạng tiền tệ
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "URL Hình ảnh không được vượt quá 1000 ký tự.")]
        [Display(Name = "URL Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Còn bán")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Món đặc biệt trong ngày")]
        public bool IsSpecialOfTheDay { get; set; } = false;

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm.")]
        [Display(Name = "Số lượng tồn kho")]
        public int StockQuantity { get; set; } = 0; // Mặc định là 0

        // Khóa ngoại tới FoodCategory
        [Required(ErrorMessage = "Danh mục món ăn là bắt buộc.")]
        [Display(Name = "Danh mục")]
        public int FoodCategoryCategoryId { get; set; }

        [ForeignKey("FoodCategoryCategoryId")]
        public virtual FoodCategory? FoodCategory { get; set; }

        // Khóa ngoại tới Canteen
        [Required(ErrorMessage = "Nhà ăn là bắt buộc.")]
        [Display(Name = "Nhà ăn")]
        public int CanteenId { get; set; }

        [ForeignKey("CanteenId")]
        public virtual Canteen? Canteen { get; set; }

        // Navigation property cho các mối quan hệ nhiều-nhiều
        public virtual ICollection<MenuItem>? MenuItems { get; set; }
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}