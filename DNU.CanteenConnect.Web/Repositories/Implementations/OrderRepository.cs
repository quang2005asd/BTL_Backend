// Path: DNU.CanteenConnect.Web/Repositories/OrderRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Thêm using cho Linq
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai OrderRepository
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy Order kèm theo User, Canteen và OrderItems (eager loading).
        /// </summary>
        /// <param name="id">ID của Order.</param>
        /// <returns>Order với thông tin chi tiết hoặc null.</returns>
        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(o => o.User) // Load User liên quan
                .Include(o => o.Canteen) // Load Canteen liên quan
                .Include(o => o.OrderItems) // Load OrderItems liên quan
                    .ThenInclude(oi => oi.FoodItem) // Load FoodItem bên trong OrderItems
                .FirstOrDefaultAsync(o => o.OrderId == id); // Tìm theo OrderId
        }

        /// <summary>
        /// Lấy tất cả Order kèm theo User, Canteen và OrderItems (eager loading).
        /// </summary>
        /// <returns>Tập hợp Order với thông tin chi tiết.</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _dbSet
                .Include(o => o.User) // Load User liên quan
                .Include(o => o.Canteen) // Load Canteen liên quan
                .Include(o => o.OrderItems) // Load OrderItems liên quan
                    .ThenInclude(oi => oi.FoodItem) // Load FoodItem bên trong OrderItems
                .ToListAsync();
        }

        /// <summary>
        /// Lấy các đơn hàng của một người dùng cụ thể.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Tập hợp các đơn hàng của người dùng.</returns>
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _dbSet
                .Include(o => o.Canteen) // Tùy chọn: load thông tin Canteen
                .Include(o => o.OrderItems) // Load OrderItems
                    .ThenInclude(oi => oi.FoodItem) // Load FoodItem bên trong OrderItems
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate) // Sắp xếp theo ngày đặt hàng mới nhất
                .ToListAsync();
        }
    }
}
