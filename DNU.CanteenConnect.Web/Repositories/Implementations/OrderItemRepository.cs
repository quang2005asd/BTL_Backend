// Path: DNU.CanteenConnect.Web/Repositories/OrderItemRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai OrderItemRepository
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy OrderItem kèm theo Order và FoodItem (eager loading).
        /// </summary>
        /// <param name="id">ID của OrderItem.</param>
        /// <returns>OrderItem với thông tin chi tiết hoặc null.</returns>
        public async Task<OrderItem?> GetOrderItemWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(oi => oi.Order) // Load Order liên quan
                .Include(oi => oi.FoodItem) // Load FoodItem liên quan
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id); // Tìm theo OrderItemId
        }

        /// <summary>
        /// Lấy tất cả OrderItem kèm theo Order và FoodItem (eager loading).
        /// </summary>
        /// <returns>Tập hợp OrderItem với thông tin chi tiết.</returns>
        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsWithDetailsAsync()
        {
            return await _dbSet
                .Include(oi => oi.Order) // Load Order liên quan
                .Include(oi => oi.FoodItem) // Load FoodItem liên quan
                .ToListAsync();
        }
    }
}