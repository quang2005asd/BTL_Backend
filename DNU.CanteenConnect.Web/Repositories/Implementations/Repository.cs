// Path: DNU.CanteenConnect.Web/Repositories/Repository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System; // Cần thiết cho Func<TEntity, bool>

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai chung cho IRepository
    // TEntity: Loại thực thể (Model)
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        // Protected để các lớp con có thể truy cập
        protected readonly ApplicationDbContext _context;
        // DbSet để tương tác với bảng cụ thể trong CSDL
        protected readonly DbSet<TEntity> _dbSet;

        // Constructor nhận vào ApplicationDbContext
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>(); // Khởi tạo DbSet cho TEntity cụ thể
        }

        /// <summary>
        /// Thêm một thực thể mới vào CSDL.
        /// </summary>
        /// <param name="entity">Thực thể cần thêm.</param>
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Thêm nhiều thực thể mới vào CSDL.
        /// </summary>
        /// <param name="entities">Danh sách các thực thể cần thêm.</param>
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Tìm kiếm các thực thể dựa trên một biểu thức điều kiện.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện.</param>
        /// <returns>Một tập hợp các thực thể thỏa mãn điều kiện.</returns>
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Lấy tất cả các thực thể từ CSDL.
        /// </summary>
        /// <returns>Một tập hợp tất cả các thực thể.</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Lấy một thực thể theo ID số nguyên.
        /// </summary>
        /// <param name="id">ID của thực thể.</param>
        /// <returns>Thực thể tìm thấy hoặc null nếu không tìm thấy.</returns>
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Lấy một thực thể theo ID chuỗi (thường dùng cho IdentityUser).
        /// </summary>
        /// <param name="id">ID chuỗi của thực thể.</param>
        /// <returns>Thực thể tìm thấy hoặc null nếu không tìm thấy.</returns>
        public async Task<TEntity?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Cập nhật một thực thể trong CSDL.
        /// </summary>
        /// <param name="entity">Thực thể cần cập nhật.</param>
        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            // Vì _dbSet.Update chỉ đánh dấu trạng thái, không có thao tác bất đồng bộ nào cần chờ.
            // Do đó, chúng ta trả về một Task đã hoàn thành.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Xóa một thực thể khỏi CSDL dựa trên ID số nguyên.
        /// </summary>
        /// <param name="id">ID của thực thể cần xóa.</param>
        public async Task DeleteAsync(int id)
        {
            // Tìm thực thể theo ID
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            // Nếu không tìm thấy, không làm gì cả (có thể thêm logging nếu cần)
        }

        /// <summary>
        /// Xóa một thực thể khỏi CSDL dựa trên ID chuỗi.
        /// </summary>
        /// <param name="id">ID chuỗi của thực thể cần xóa.</param>
        public async Task DeleteAsync(string id)
        {
            // Tìm thực thể theo ID chuỗi
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            // Nếu không tìm thấy, không làm gì cả
        }

        /// <summary>
        /// Lưu các thay đổi vào CSDL.
        /// </summary>
        /// <returns>Số lượng bản ghi đã bị ảnh hưởng.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
