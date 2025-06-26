// ICanteenService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using DNU.CanteenConnect.Web.Models; // Đảm bảo namespace này khớp với vị trí của Models

namespace DNU.CanteenConnect.Web.Services.Interfaces // Đã điều chỉnh namespace
{
    // Giao diện định nghĩa các hoạt động dịch vụ cho thực thể Canteen.
    public interface ICanteenService
    {
        // Lấy tất cả các Canteen.
        Task<IEnumerable<Canteen>> GetAllCanteensAsync();

        // Lấy một Canteen theo ID.
        Task<Canteen> GetCanteenByIdAsync(int id);

        // Thêm một Canteen mới.
        Task AddCanteenAsync(Canteen canteen);

        // Cập nhật thông tin một Canteen hiện có.
        Task UpdateCanteenAsync(Canteen canteen);

        // Xóa một Canteen theo ID.
        Task DeleteCanteenAsync(int id);

        // Kiểm tra xem Canteen có tồn tại không.
        Task<bool> CanteenExistsAsync(int id);
    }
}