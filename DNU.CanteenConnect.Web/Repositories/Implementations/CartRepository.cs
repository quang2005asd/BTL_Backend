// Path: DNU.CanteenConnect.Web/Repositories/CartRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai CartRepository
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy giỏ hàng của một người dùng cụ thể, kèm theo CartItems và FoodItems.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Giỏ hàng của người dùng hoặc null.</returns>
        public async Task<Cart?> GetCartByUserIdWithDetailsAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.CartItems) // Load CartItems liên quan
                .ThenInclude(ci => ci.FoodItem) // Load FoodItem bên trong CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId); // Tìm theo UserId
        }
    }
}