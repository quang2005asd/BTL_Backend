// DNU.CanteenConnect.Web/Models/HomeViewModel.cs
using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Models
{
    public class HomeViewModel
    {
        // === ĐÃ SỬA LẠI KIỂU DỮ LIỆU Ở ĐÂY ===
        public List<MenuItemViewModel> FeaturedFoodItems { get; set; } = new List<MenuItemViewModel>();

        // Bạn có thể thêm các thuộc tính khác ở đây nếu cần cho trang chủ
    }
}