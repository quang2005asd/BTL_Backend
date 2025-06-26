// Path: DNU.CanteenConnect.Web/Interfaces/IFoodItemRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho FoodItemRepository, kế thừa từ IRepository chung
    public interface IFoodItemRepository : IRepository<FoodItem>
    {
        // Lấy FoodItem kèm theo FoodCategory và Canteen (eager loading)
        Task<FoodItem?> GetFoodItemWithDetailsAsync(int id);
        // Lấy tất cả FoodItem kèm theo FoodCategory và Canteen
        Task<IEnumerable<FoodItem>> GetAllFoodItemsWithDetailsAsync();
    }
}