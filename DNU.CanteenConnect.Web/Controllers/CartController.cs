// DNU.CanteenConnect.Web/Controllers/CartController.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize] // Yêu cầu người dùng đăng nhập để sử dụng giỏ hàng
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Nếu người dùng chưa đăng nhập, trả về giỏ hàng rỗng hoặc chuyển hướng đến trang đăng nhập
                // Đối với giỏ hàng, thường là giỏ hàng trống nếu chưa đăng nhập hoặc không tìm thấy user ID.
                return View(new Cart { CartItems = new List<CartItem>() });
            }

            var cart = await _context.Carts
                                     .Include(c => c.CartItems)!
                                         .ThenInclude(ci => ci.FoodItem)!
                                             .ThenInclude(fi => fi.FoodCategory) // Bao gồm cả FoodCategory
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            // Nếu giỏ hàng không tồn tại, tạo một giỏ hàng trống để tránh lỗi null reference
            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
            }

            return View(cart);
        }

        // GET: /Cart/GetCartItemCount
        // API endpoint để lấy số lượng món trong giỏ hàng (dùng cho Navbar)
        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            int count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            return Json(new { count = count });
        }


        // POST: /Cart/AddToCart
        // Thêm một món ăn vào giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF
        public async Task<IActionResult> AddToCart(int itemId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm món ăn vào giỏ hàng." });
            }

            var foodItem = await _context.FoodItems.FindAsync(itemId);
            if (foodItem == null)
            {
                return Json(new { success = false, message = "Món ăn không tồn tại." });
            }

            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Nếu người dùng chưa có giỏ hàng, tạo một giỏ hàng mới
                cart = new Cart { UserId = userId, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Lưu để có CartId
            }
            else
            {
                cart.LastModifiedDate = DateTime.Now;
            }

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.FoodItemId == itemId);

            if (cartItem != null)
            {
                // Nếu món ăn đã có trong giỏ, tăng số lượng
                cartItem.Quantity += quantity;
                // Cập nhật giá tại thời điểm thêm vào giỏ hàng (nếu giá món ăn có thể thay đổi)
                cartItem.PriceAtAddition = foodItem.Price; 
            }
            else
            {
                // Nếu món ăn chưa có trong giỏ, thêm mới
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    FoodItemId = itemId,
                    Quantity = quantity,
                    PriceAtAddition = foodItem.Price
                };
                cart.CartItems?.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Tính tổng số lượng món trong giỏ để cập nhật navbar
            var totalItemsInCart = await _context.CartItems
                                                .Where(ci => ci.CartId == cart.CartId)
                                                .SumAsync(ci => ci.Quantity);

            return Json(new { success = true, message = $"Đã thêm {foodItem.Name} vào giỏ hàng.", totalItems = totalItemsInCart });
        }

        // POST: /Cart/UpdateCartItemQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để cập nhật giỏ hàng." });
            }

            var cartItem = await _context.CartItems
                                        .Include(ci => ci.Cart)
                                        .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Món ăn trong giỏ không tìm thấy." });
            }

            if (newQuantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = newQuantity;
            }
            await _context.SaveChangesAsync();

            // Cập nhật ngày sửa đổi cuối cùng của giỏ hàng
            var cart = await _context.Carts.FindAsync(cartItem.CartId);
            if (cart != null)
            {
                cart.LastModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Số lượng đã được cập nhật." });
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để xóa món ăn khỏi giỏ hàng." });
            }

            var cartItem = await _context.CartItems
                                        .Include(ci => ci.Cart)
                                        .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Món ăn trong giỏ không tìm thấy." });
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Cập nhật ngày sửa đổi cuối cùng của giỏ hàng
            var cart = await _context.Carts.FindAsync(cartItem.CartId);
            if (cart != null)
            {
                cart.LastModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Đã xóa món ăn khỏi giỏ hàng." });
        }
    }
}
