// Path: DNU.CanteenConnect.Web/Repositories/CartItemRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai CartItemRepository
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy CartItem kèm theo Cart và FoodItem (eager loading).
        /// </summary>
        /// <param name="id">ID của CartItem.</param>
        /// <returns>CartItem với thông tin chi tiết hoặc null.</returns>
        public async Task<CartItem?> GetCartItemWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(ci => ci.Cart) // Load Cart liên quan
                .Include(ci => ci.FoodItem) // Load FoodItem liên quan
                .FirstOrDefaultAsync(ci => ci.CartItemId == id); // Tìm theo CartItemId
        }

        /// <summary>
        /// Lấy tất cả CartItem kèm theo Cart và FoodItem (eager loading).
        /// </summary>
        /// <returns>Tập hợp CartItem với thông tin chi tiết.</returns>
        public async Task<IEnumerable<CartItem>> GetAllCartItemsWithDetailsAsync()
        {
            return await _dbSet
                .Include(ci => ci.Cart) // Load Cart liên quan
                .Include(ci => ci.FoodItem) // Load FoodItem liên quan
                .ToListAsync();
        }
    }
}