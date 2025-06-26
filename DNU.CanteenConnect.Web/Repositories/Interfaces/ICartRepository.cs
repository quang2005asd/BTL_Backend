// Path: DNU.CanteenConnect.Web/Interfaces/ICartRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho CartRepository, kế thừa từ IRepository chung
    public interface ICartRepository : IRepository<Cart>
    {
        // Lấy giỏ hàng của một người dùng cụ thể, kèm theo CartItems và FoodItems
        Task<Cart?> GetCartByUserIdWithDetailsAsync(string userId);
    }
}