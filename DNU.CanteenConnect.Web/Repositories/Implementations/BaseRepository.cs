// BaseRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DNU.CanteenConnect.Web.Data; // Đảm bảo namespace này khớp với cấu trúc dự án của bạn
using DNU.CanteenConnect.Web.Repositories.Interfaces; // Đảm bảo namespace này khớp với cấu trúc dự án của bạn

namespace DNU.CanteenConnect.Web.Repositories.Implementations
{
    // Lớp cơ sở triển khai các hoạt động Repository chung cho tất cả các thực thể.
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        // Constructor để inject ApplicationDbContext.
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Lấy tất cả các thực thể.
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Lấy một thực thể theo ID.
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Thêm một thực thể mới.
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync(); // Lưu thay đổi ngay sau khi thêm
        }

        // Cập nhật một thực thể hiện có.
        public async Task UpdateAsync(T entity) // <-- Triển khai phương thức này
        {
            _dbSet.Update(entity);
            await SaveChangesAsync(); // Lưu thay đổi ngay sau khi cập nhật
        }

        // Xóa một thực thể theo ID.
        public async Task DeleteAsync(int id) // <-- Triển khai phương thức này
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await SaveChangesAsync(); // Lưu thay đổi ngay sau khi xóa
            }
        }

        // Lưu các thay đổi vào cơ sở dữ liệu.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
