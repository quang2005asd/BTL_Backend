// DNU.CanteenConnect.Web/Models/Dish.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNU.CanteenConnect.Web.Models // Rất quan trọng: Đảm bảo namespace này khớp!
{
    public class Dish
    {
        [Key] // Khóa chính
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên món ăn là bắt buộc.")] // Tên món ăn không được để trống
        [StringLength(100, ErrorMessage = "Tên món ăn không được quá 100 ký tự.")] // Giới hạn độ dài
        [Display(Name = "Tên món ăn")] // Tên hiển thị trên UI
        public string Name { get; set; } = string.Empty; // Khởi tạo giá trị mặc định để tránh null warnings

        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; } // Có thể để trống

        [Required(ErrorMessage = "Giá tiền là bắt buộc.")]
        [Column(TypeName = "decimal(18, 2)")] // Định dạng kiểu số thập phân cho cơ sở dữ liệu
        [Range(0.01, 1000000.00, ErrorMessage = "Giá tiền phải lớn hơn 0.")] // Giới hạn giá trị
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "URL ảnh")]
        [DataType(DataType.ImageUrl)] // Gợi ý kiểu dữ liệu là URL hình ảnh
        public string? ImageUrl { get; set; }

        [Display(Name = "Còn hàng")]
        public bool IsAvailable { get; set; } = true; // Mặc định là món ăn còn hàng
    }
}