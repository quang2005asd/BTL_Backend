// Path: DNU.CanteenConnect.Web/Interfaces/IDailyMenuRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho DailyMenuRepository, kế thừa từ IRepository chung
    public interface IDailyMenuRepository : IRepository<DailyMenu>
    {
        // Lấy DailyMenu kèm theo Canteen và MenuItems (eager loading)
        Task<DailyMenu?> GetDailyMenuWithDetailsAsync(int id);

        // Lấy DailyMenu theo ngày và CanteenId
        Task<DailyMenu?> GetDailyMenuByDateAndCanteenAsync(DateTime date, int canteenId);

        // Lấy tất cả DailyMenu kèm theo Canteen và MenuItems
        Task<IEnumerable<DailyMenu>> GetAllDailyMenusWithDetailsAsync();
    }
}