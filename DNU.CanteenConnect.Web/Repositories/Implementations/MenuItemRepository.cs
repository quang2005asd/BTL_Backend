// Path: DNU.CanteenConnect.Web/Repositories/MenuItemRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai MenuItemRepository
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy MenuItem kèm theo DailyMenu và FoodItem (eager loading).
        /// </summary>
        /// <param name="id">ID của MenuItem.</param>
        /// <returns>MenuItem với thông tin chi tiết hoặc null.</returns>
        public async Task<MenuItem?> GetMenuItemWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(mi => mi.DailyMenu) // Load DailyMenu liên quan
                .Include(mi => mi.FoodItem) // Load FoodItem liên quan
                .FirstOrDefaultAsync(mi => mi.MenuItemId == id); // Tìm theo MenuItemId
        }

        /// <summary>
        /// Lấy tất cả MenuItem kèm theo DailyMenu và FoodItem (eager loading).
        /// </summary>
        /// <returns>Tập hợp MenuItem với thông tin chi tiết.</returns>
        public async Task<IEnumerable<MenuItem>> GetAllMenuItemsWithDetailsAsync()
        {
            return await _dbSet
                .Include(mi => mi.DailyMenu) // Load DailyMenu liên quan
                .Include(mi => mi.FoodItem) // Load FoodItem liên quan
                .ToListAsync();
        }
    }
}