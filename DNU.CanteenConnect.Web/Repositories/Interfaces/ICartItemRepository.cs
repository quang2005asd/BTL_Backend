// Path: DNU.CanteenConnect.Web/Interfaces/ICartItemRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho CartItemRepository, kế thừa từ IRepository chung
    public interface ICartItemRepository : IRepository<CartItem>
    {
        // Lấy CartItem kèm theo Cart và FoodItem
        Task<CartItem?> GetCartItemWithDetailsAsync(int id);
        // Lấy tất cả CartItem kèm theo Cart và FoodItem
        Task<IEnumerable<CartItem>> GetAllCartItemsWithDetailsAsync();
    }
}