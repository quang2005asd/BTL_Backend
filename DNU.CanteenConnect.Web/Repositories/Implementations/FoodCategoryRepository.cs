// Path: DNU.CanteenConnect.Web/Repositories/FoodCategoryRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai FoodCategoryRepository
    public class FoodCategoryRepository : Repository<FoodCategory>, IFoodCategoryRepository
    {
        public FoodCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Triển khai các phương thức đặc thù của IFoodCategoryRepository nếu có
    }
}