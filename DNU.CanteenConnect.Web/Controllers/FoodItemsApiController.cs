
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Cần thiết để sử dụng ToListAsync(), Include(), v.v.
using DNU.CanteenConnect.Web.Data;     // Namespace của ApplicationDbContext
using DNU.CanteenConnect.Web.Models;   // Namespace của các Models của bạn
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    // Đánh dấu đây là một API Controller
    [Route("api/[controller]")] // Định tuyến cơ bản: /api/FoodItemsApi
    [ApiController]             // Cung cấp các tính năng hữu ích cho API Controller (tự động validate, xử lý lỗi)
    public class FoodItemsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor để "tiêm" ApplicationDbContext vào Controller
        public FoodItemsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FoodItemsApi
        // Lấy danh sách tất cả món ăn, có thể lọc theo category và tìm kiếm theo từ khóa.
        // Ví dụ: GET /api/FoodItemsApi?categoryName=Món Chính&searchTerm=cơm
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems(
            [FromQuery] string? categoryName, // Lọc theo tên danh mục
            [FromQuery] string? searchTerm)   // Tìm kiếm theo tên hoặc mô tả
        {
            IQueryable<FoodItem> query = _context.FoodItems
                                                .Include(f => f.FoodCategory) // Bao gồm thông tin danh mục
                                                .Include(f => f.Canteen);     // Bao gồm thông tin nhà ăn

            // Lọc theo Category Name (nếu có)
            if (!string.IsNullOrEmpty(categoryName) && categoryName != "Tất cả")
            {
                query = query.Where(f => f.FoodCategory != null && f.FoodCategory.Name == categoryName);
            }

            // Lọc theo Search Term (nếu có)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string lowerCaseSearchTerm = searchTerm.ToLower();
                query = query.Where(f =>
                    f.Name.ToLower().Contains(lowerCaseSearchTerm) ||
                    (f.Description != null && f.Description.ToLower().Contains(lowerCaseSearchTerm)));
            }

            // Trả về danh sách món ăn đã lọc
            return await query.ToListAsync();
        }

        // GET: api/FoodItemsApi/5
        // Lấy thông tin một món ăn cụ thể bằng ItemId
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItem(int id)
        {
            var foodItem = await _context.FoodItems
                                        .Include(f => f.FoodCategory)
                                        .Include(f => f.Canteen)
                                        .FirstOrDefaultAsync(f => f.ItemId == id);

            if (foodItem == null)
            {
                return NotFound(); // Trả về HTTP 404 nếu không tìm thấy
            }

            return foodItem; // Trả về món ăn
        }

        // POST: api/FoodItemsApi
        // Thêm một món ăn mới
        [HttpPost]
        public async Task<ActionResult<FoodItem>> PostFoodItem(FoodItem foodItem)
        {
            // Kiểm tra xem Id của món ăn có phải là 0 không (để thêm mới)
            if (foodItem.ItemId != 0)
            {
                return BadRequest("ItemId should not be set for a new food item.");
            }

            _context.FoodItems.Add(foodItem);
            await _context.SaveChangesAsync();

            // Trả về HTTP 201 Created và thông tin món ăn vừa tạo
            return CreatedAtAction(nameof(GetFoodItem), new { id = foodItem.ItemId }, foodItem);
        }

        // PUT: api/FoodItemsApi/5
        // Cập nhật thông tin một món ăn hiện có
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodItem(int id, FoodItem foodItem)
        {
            if (id != foodItem.ItemId)
            {
                return BadRequest(); // Trả về HTTP 400 nếu ID không khớp
            }

            _context.Entry(foodItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodItemExists(id))
                {
                    return NotFound(); // Trả về HTTP 404 nếu món ăn không tồn tại
                }
                else
                {
                    throw; // Ném lỗi nếu có vấn đề khác
                }
            }

            return NoContent(); // Trả về HTTP 204 No Content nếu cập nhật thành công
        }

        // DELETE: api/FoodItemsApi/5
        // Xóa một món ăn
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound(); // Trả về HTTP 404 nếu không tìm thấy món ăn
            }

            _context.FoodItems.Remove(foodItem);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về HTTP 204 No Content nếu xóa thành công
        }

        // Kiểm tra xem món ăn có tồn tại không
        private bool FoodItemExists(int id)
        {
            return _context.FoodItems.Any(e => e.ItemId == id);
        }
    }
}