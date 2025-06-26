
// Path: DNU.CanteenConnect.Web/Controllers/FoodCategoriesApiController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Route("api/[controller]")] // Định tuyến cơ bản: /api/FoodCategoriesApi
    [ApiController]
    public class FoodCategoriesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FoodCategoriesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FoodCategoriesApi
        // Lấy danh sách tất cả các danh mục món ăn
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodCategory>>> GetFoodCategories()
        {
            return await _context.FoodCategories.ToListAsync();
        }

        // GET: api/FoodCategoriesApi/5
        // Lấy thông tin một danh mục món ăn cụ thể bằng CategoryId
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodCategory>> GetFoodCategory(int id)
        {
            var foodCategory = await _context.FoodCategories.FindAsync(id);

            if (foodCategory == null)
            {
                return NotFound(); // Trả về HTTP 404 nếu không tìm thấy
            }

            return foodCategory; // Trả về danh mục món ăn
        }

        // POST: api/FoodCategoriesApi
        // Thêm một danh mục món ăn mới
        [HttpPost]
        public async Task<ActionResult<FoodCategory>> PostFoodCategory(FoodCategory foodCategory)
        {
            // Kiểm tra xem Id của danh mục có phải là 0 không (để thêm mới)
            if (foodCategory.CategoryId != 0)
            {
                return BadRequest("CategoryId should not be set for a new food category.");
            }

            _context.FoodCategories.Add(foodCategory);
            await _context.SaveChangesAsync();

            // Trả về HTTP 201 Created và thông tin danh mục vừa tạo
            return CreatedAtAction(nameof(GetFoodCategory), new { id = foodCategory.CategoryId }, foodCategory);
        }

        // PUT: api/FoodCategoriesApi/5
        // Cập nhật thông tin một danh mục món ăn hiện có
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodCategory(int id, FoodCategory foodCategory)
        {
            if (id != foodCategory.CategoryId)
            {
                return BadRequest(); // Trả về HTTP 400 nếu ID không khớp
            }

            _context.Entry(foodCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodCategoryExists(id))
                {
                    return NotFound(); // Trả về HTTP 404 nếu danh mục không tồn tại
                }
                else
                {
                    throw; // Ném lỗi nếu có vấn đề khác
                }
            }

            return NoContent(); // Trả về HTTP 204 No Content nếu cập nhật thành công
        }

        // DELETE: api/FoodCategoriesApi/5
        // Xóa một danh mục món ăn
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodCategory(int id)
        {
            var foodCategory = await _context.FoodCategories.FindAsync(id);
            if (foodCategory == null)
            {
                return NotFound(); // Trả về HTTP 404 nếu không tìm thấy danh mục
            }

            _context.FoodCategories.Remove(foodCategory);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về HTTP 204 No Content nếu xóa thành công
        }

        // Kiểm tra xem danh mục có tồn tại không
        private bool FoodCategoryExists(int id)
        {
            return _context.FoodCategories.Any(e => e.CategoryId == id);
        }
    }
}
