// CanteenService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models; // Đảm bảo namespace này khớp với vị trí của Models
using DNU.CanteenConnect.Web.Repositories.Interfaces; // Đảm bảo namespace này khớp với vị trí của Repositories.Interfaces
using DNU.CanteenConnect.Web.Services.Interfaces; // Đã điều chỉnh namespace

namespace DNU.CanteenConnect.Web.Services.Implementations // Đã điều chỉnh namespace
{
    // Lớp triển khai các hoạt động dịch vụ cho thực thể Canteen.
    public class CanteenService : ICanteenService
    {
        private readonly ICanteenRepository _canteenRepository;

        // Constructor để inject ICanteenRepository.
        public CanteenService(ICanteenRepository canteenRepository)
        {
            _canteenRepository = canteenRepository;
        }

        // Lấy tất cả các Canteen.
        public async Task<IEnumerable<Canteen>> GetAllCanteensAsync()
        {
            return await _canteenRepository.GetAllAsync();
        }

        // Lấy một Canteen theo ID.
        public async Task<Canteen> GetCanteenByIdAsync(int id)
        {
            return await _canteenRepository.GetByIdAsync(id);
        }

        // Thêm một Canteen mới.
        public async Task AddCanteenAsync(Canteen canteen)
        {
            await _canteenRepository.AddAsync(canteen);
        }

        // Cập nhật thông tin một Canteen hiện có.
        public async Task UpdateCanteenAsync(Canteen canteen)
        {
            await _canteenRepository.UpdateAsync(canteen);
        }

        // Xóa một Canteen theo ID.
        public async Task DeleteCanteenAsync(int id)
        {
            // Có thể thêm logic kiểm tra nghiệp vụ trước khi xóa ở đây.
            await _canteenRepository.DeleteAsync(id);
        }

        // Kiểm tra xem Canteen có tồn tại không.
        public async Task<bool> CanteenExistsAsync(int id)
        {
            // Phương thức này có thể được tối ưu hóa trong Repository nếu cần.
            // Hiện tại sẽ lấy toàn bộ và kiểm tra, hoặc thêm phương thức ExistsAsync trong Repository.
            var canteen = await _canteenRepository.GetByIdAsync(id);
            return canteen != null;
        }
    }
}
