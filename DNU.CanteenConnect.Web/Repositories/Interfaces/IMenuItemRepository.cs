// Path: DNU.CanteenConnect.Web/Interfaces/IMenuItemRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho MenuItemRepository, kế thừa từ IRepository chung
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        // Lấy MenuItem kèm theo DailyMenu và FoodItem
        Task<MenuItem?> GetMenuItemWithDetailsAsync(int id);
        // Lấy tất cả MenuItem kèm theo DailyMenu và FoodItem
        Task<IEnumerable<MenuItem>> GetAllMenuItemsWithDetailsAsync();
    }
}