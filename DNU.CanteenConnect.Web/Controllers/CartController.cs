using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Helpers; // Helper để đọc/ghi object vào Session
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
    // Bỏ [Authorize] ở cấp độ class để khách có thể truy cập
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action này giờ hoạt động cho cả khách và người dùng
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Cart cart;

            if (userId != null)
            {
                // Nếu đã đăng nhập, lấy giỏ hàng từ DB (giữ nguyên logic của bạn)
                cart = await _context.Carts
                                     .Include(c => c.CartItems)!
                                         .ThenInclude(ci => ci.FoodItem)!
                                             .ThenInclude(fi => fi.FoodCategory)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            else
            {
                // Nếu là khách, lấy giỏ hàng từ Session
                cart = HttpContext.Session.Get<Cart>("GuestCart");
            }

            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
            }

            return View(cart);
        }

        // Action này giờ hoạt động cho cả khách và người dùng
        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int count = 0;

            if (userId != null)
            {
                var cart = await _context.Carts
                                         .Include(c => c.CartItems)
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
                count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            }
            else
            {
                var cart = HttpContext.Session.Get<Cart>("GuestCart");
                count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            }
            return Json(new { count });
        }

        // Action này giờ hoạt động cho cả khách và người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int itemId, int quantity = 1)
        {
            var foodItem = await _context.FoodItems.FindAsync(itemId);
            if (foodItem == null || !foodItem.IsAvailable)
            {
                return Json(new { success = false, message = "Món ăn không tồn tại hoặc đã hết hàng." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId != null)
            {
                // LOGIC CHO NGƯỜI DÙNG ĐÃ ĐĂNG NHẬP (LƯU VÀO DB)
                var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CreatedDate = DateTime.Now, CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                }
                var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.FoodItemId == itemId);
                if (cartItem != null) {
                    cartItem.Quantity += quantity;
                } else {
                    cart.CartItems?.Add(new CartItem { FoodItemId = itemId, Quantity = quantity, PriceAtAddition = foodItem.Price });
                }
                cart.LastModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else
            {
                // LOGIC MỚI CHO KHÁCH (LƯU VÀO SESSION)
                var cart = HttpContext.Session.Get<Cart>("GuestCart") ?? new Cart { CartItems = new List<CartItem>() };
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.FoodItemId == itemId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    var foodItemForSession = await _context.FoodItems.AsNoTracking().Include(fi => fi.FoodCategory).FirstOrDefaultAsync(fi => fi.ItemId == itemId);
                    cart.CartItems.Add(new CartItem
                    {
                        FoodItemId = foodItem.ItemId,
                        FoodItem = foodItemForSession,
                        Quantity = quantity,
                        PriceAtAddition = foodItem.Price
                    });
                }
                HttpContext.Session.Set("GuestCart", cart);
            }

            var totalItemsInCart = await GetCartItemCountValue();
            return Json(new { success = true, message = $"Đã thêm '{foodItem.Name}' vào giỏ hàng.", totalItems = totalItemsInCart });
        }
        
        // GIỮ NGUYÊN CÁC ACTION UPDATE/REMOVE CỦA BẠN VÀ THÊM [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Json(new { success = false, message = "Bạn cần đăng nhập để cập nhật giỏ hàng." });
            
            var cartItem = await _context.CartItems.Include(ci => ci.Cart).FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart!.UserId == userId);
            if (cartItem == null) return Json(new { success = false, message = "Món ăn trong giỏ không tìm thấy." });

            if (newQuantity <= 0) _context.CartItems.Remove(cartItem);
            else cartItem.Quantity = newQuantity;
            
            var cart = await _context.Carts.FindAsync(cartItem.CartId);
            if (cart != null) cart.LastModifiedDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Số lượng đã được cập nhật." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Json(new { success = false, message = "Bạn cần đăng nhập để xóa món ăn khỏi giỏ hàng." });

            var cartItem = await _context.CartItems.Include(ci => ci.Cart).FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart!.UserId == userId);
            if (cartItem == null) return Json(new { success = false, message = "Món ăn trong giỏ không tìm thấy." });

            _context.CartItems.Remove(cartItem);
            var cart = await _context.Carts.FindAsync(cartItem.CartId);
            if (cart != null) cart.LastModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xóa món ăn khỏi giỏ hàng." });
        }

        // Hàm private để tính toán lại số lượng item trong giỏ
        private async Task<int> GetCartItemCountValue()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int count = 0;
            if (userId != null)
            {
                var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
                count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            }
            else
            {
                var cart = HttpContext.Session.Get<Cart>("GuestCart");
                count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            }
            return count;
        }
    }
}