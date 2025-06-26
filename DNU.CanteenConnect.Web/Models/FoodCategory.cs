// DNU.CanteenConnect.Web/Models/FoodCategory.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DNU.CanteenConnect.Web.Models
{
    public class FoodCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên danh mục không được vượt quá 255 ký tự.")]
        [Display(Name = "Tên Danh mục")]
        public string Name { get; set; } = default!;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        // Thêm 'virtual' để kích hoạt Lazy Loading
        public virtual ICollection<FoodItem>? FoodItems { get; set; }
    }
}