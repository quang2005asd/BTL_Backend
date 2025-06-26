// DNU.CanteenConnect.Web/Models/FoodItem.cs
using System.Collections.Generic; // THÊM using này nếu chưa có
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

        [Display(Name = "Có sẵn")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Món đặc biệt trong ngày")]
        public bool IsSpecialOfTheDay { get; set; } = false;

        // --- GIỮ NGUYÊN CÁCH VIẾT CỦA BẠN ---
        [Required(ErrorMessage = "Danh mục món ăn là bắt buộc.")]
        [Display(Name = "Danh mục")]
        public int FoodCategoryCategoryId { get; set; }

        [ForeignKey("FoodCategoryCategoryId")]
        public virtual FoodCategory? FoodCategory { get; set; } // Chỉ thêm 'virtual'

        // --- GIỮ NGUYÊN CÁCH VIẾT CỦA BẠN ---
        [Required(ErrorMessage = "Nhà ăn là bắt buộc.")]
        [Display(Name = "Nhà ăn")]
        public int CanteenId { get; set; }

        [ForeignKey("CanteenId")]
        public virtual Canteen? Canteen { get; set; } // Chỉ thêm 'virtual'

        // --- CẬP NHẬT: THÊM 'virtual' ---
        public virtual ICollection<MenuItem>? MenuItems { get; set; }

        // --- THÊM MỚI: DÒNG QUAN TRỌNG NHẤT ĐỂ SỬA LỖI ---
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}