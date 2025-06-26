// IBaseRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories.Interfaces // Đảm bảo namespace này khớp với cấu trúc dự án của bạn
{
    // Giao diện cơ sở cho tất cả các Repository, định nghĩa các hoạt động CRUD chung.
    public interface IBaseRepository<T> where T : class
    {
        // Lấy tất cả các thực thể.
        Task<IEnumerable<T>> GetAllAsync();

        // Lấy một thực thể theo ID.
        Task<T> GetByIdAsync(int id);

        // Thêm một thực thể mới.
        Task AddAsync(T entity);

        // Cập nhật một thực thể hiện có.
        Task UpdateAsync(T entity); // <-- Đã thêm phương thức này

        // Xóa một thực thể theo ID.
        Task DeleteAsync(int id); // <-- Đã thêm phương thức này

        // Lưu các thay đổi vào cơ sở dữ liệu.
        Task SaveChangesAsync();
    }
}