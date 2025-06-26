// Path: DNU.CanteenConnect.Web/Repositories/DailyMenuRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; // Thêm using cho Linq
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai DailyMenuRepository
    public class DailyMenuRepository : Repository<DailyMenu>, IDailyMenuRepository
    {
        public DailyMenuRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy DailyMenu kèm theo Canteen và MenuItems (eager loading).
        /// </summary>
        /// <param name="id">ID của DailyMenu.</param>
        /// <returns>DailyMenu với thông tin chi tiết hoặc null.</returns>
        public async Task<DailyMenu?> GetDailyMenuWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(dm => dm.Canteen) // Load Canteen liên quan
                .Include(dm => dm.MenuItems) // Load MenuItems liên quan
                    .ThenInclude(mi => mi.FoodItem) // Load FoodItem bên trong MenuItems
                .FirstOrDefaultAsync(dm => dm.MenuId == id); // Tìm theo DailyMenuId
        }

        /// <summary>
        /// Lấy DailyMenu theo ngày và CanteenId.
        /// </summary>
        /// <param name="date">Ngày của DailyMenu.</param>
        /// <param name="canteenId">ID của Canteen.</param>
        /// <returns>DailyMenu tương ứng hoặc null.</returns>
        public async Task<DailyMenu?> GetDailyMenuByDateAndCanteenAsync(DateTime date, int canteenId)
        {
            return await _dbSet
                .Include(dm => dm.Canteen)
                .Include(dm => dm.MenuItems)
                    .ThenInclude(mi => mi.FoodItem)
                .FirstOrDefaultAsync(dm => dm.MenuDate.Date == date.Date && dm.CanteenId == canteenId);
        }

        /// <summary>
        /// Lấy tất cả DailyMenu kèm theo Canteen và MenuItems (eager loading).
        /// </summary>
        /// <returns>Tập hợp DailyMenu với thông tin chi tiết.</returns>
        public async Task<IEnumerable<DailyMenu>> GetAllDailyMenusWithDetailsAsync()
        {
            return await _dbSet
                .Include(dm => dm.Canteen) // Load Canteen liên quan
                .Include(dm => dm.MenuItems) // Load MenuItems liên quan
                    .ThenInclude(mi => mi.FoodItem) // Load FoodItem bên trong MenuItems
                .ToListAsync();
        }
    }
}
