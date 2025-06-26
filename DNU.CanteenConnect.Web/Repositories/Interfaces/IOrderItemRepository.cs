// Path: DNU.CanteenConnect.Web/Interfaces/IOrderItemRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho OrderItemRepository, kế thừa từ IRepository chung
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        // Lấy OrderItem kèm theo Order và FoodItem
        Task<OrderItem?> GetOrderItemWithDetailsAsync(int id);
        // Lấy tất cả OrderItem kèm theo Order và FoodItem
        Task<IEnumerable<OrderItem>> GetAllOrderItemsWithDetailsAsync();
    }
}