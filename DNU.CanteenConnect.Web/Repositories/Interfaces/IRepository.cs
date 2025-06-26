// Path: DNU.CanteenConnect.Web/Interfaces/IRepository.cs
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks; // Cần thiết cho các phương thức bất đồng bộ (Task)
using System; // Cần thiết cho Func<TEntity, bool>

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện chung cho tất cả các Repository
    // TEntity: Loại thực thể (Model) mà Repository này sẽ làm việc
    public interface IRepository<TEntity> where TEntity : class
    {
        // Lấy tất cả các thực thể
        Task<IEnumerable<TEntity>> GetAllAsync();

        // Lấy một thực thể theo ID (số nguyên)
        Task<TEntity?> GetByIdAsync(int id);

        // Lấy một thực thể theo ID (chuỗi - dùng cho IdentityUser)
        Task<TEntity?> GetByIdAsync(string id);

        // Tìm kiếm các thực thể theo điều kiện
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        // Thêm một thực thể mới
        Task AddAsync(TEntity entity);

        // Thêm nhiều thực thể mới
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        // Cập nhật một thực thể hiện có (ĐÃ THAY ĐỔI TỪ Update -> UpdateAsync)
        Task UpdateAsync(TEntity entity);

        // Xóa một thực thể theo ID (ĐÃ THAY ĐỔI TỪ Remove -> DeleteAsync, nhận ID)
        Task DeleteAsync(int id);
        
        // Xóa một thực thể theo ID (cho kiểu string - ví dụ: User ID)
        Task DeleteAsync(string id); // <-- Bổ sung cho các entities có ID kiểu string (như User)

        // Lưu các thay đổi vào cơ sở dữ liệu
        Task<int> SaveChangesAsync();
    }
}