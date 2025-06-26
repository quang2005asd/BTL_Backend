// Path: DNU.CanteenConnect.Web/Repositories/FoodItemRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai FoodItemRepository
    public class FoodItemRepository : Repository<FoodItem>, IFoodItemRepository
    {
        public FoodItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy FoodItem kèm theo FoodCategory và Canteen (eager loading).
        /// </summary>
        /// <param name="id">ID của FoodItem.</param>
        /// <returns>FoodItem với thông tin chi tiết hoặc null.</returns>
        public async Task<FoodItem?> GetFoodItemWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(fi => fi.FoodCategory) // Load FoodCategory liên quan
                .Include(fi => fi.Canteen) // Load Canteen liên quan
                .FirstOrDefaultAsync(fi => fi.ItemId == id); // Tìm theo ItemId
        }

        /// <summary>
        /// Lấy tất cả FoodItem kèm theo FoodCategory và Canteen (eager loading).
        /// </summary>
        /// <returns>Tập hợp FoodItem với thông tin chi tiết.</returns>
        public async Task<IEnumerable<FoodItem>> GetAllFoodItemsWithDetailsAsync()
        {
            return await _dbSet
                .Include(fi => fi.FoodCategory) // Load FoodCategory liên quan
                .Include(fi => fi.Canteen) // Load Canteen liên quan
                .ToListAsync();
        }
    }
}