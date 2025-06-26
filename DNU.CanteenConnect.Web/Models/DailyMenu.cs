// DNU.CanteenConnect.Web/Models/DailyMenu.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần dùng cho Column và ForeignKey

namespace DNU.CanteenConnect.Web.Models
{
    public class DailyMenu
    {
        [Key] // Đánh dấu MenuId là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Đảm bảo MenuId tự động tăng
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Ngày thực đơn là bắt buộc.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày Thực đơn")]
        public DateTime MenuDate { get; set; }

        [Display(Name = "Nhà ăn")]
        public int? CanteenId { get; set; } // Khóa ngoại tới Canteen, có thể null nếu thực đơn không thuộc nhà ăn cụ thể

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("CanteenId")]
        [Display(Name = "Nhà ăn")]
        public Canteen? Canteen { get; set; }

        // Mối quan hệ với các món ăn trong thực đơn (MenuItem)
        public ICollection<MenuItem>? MenuItems { get; set; }
    }
}