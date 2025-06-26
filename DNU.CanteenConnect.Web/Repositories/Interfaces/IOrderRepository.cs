// Path: DNU.CanteenConnect.Web/Interfaces/IOrderRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho OrderRepository, kế thừa từ IRepository chung
    public interface IOrderRepository : IRepository<Order>
    {
        // Lấy Order kèm theo User, Canteen và OrderItems (eager loading)
        Task<Order?> GetOrderWithDetailsAsync(int id);
        // Lấy tất cả Order kèm theo User, Canteen và OrderItems
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
        // Lấy các đơn hàng của một người dùng cụ thể
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    }
}