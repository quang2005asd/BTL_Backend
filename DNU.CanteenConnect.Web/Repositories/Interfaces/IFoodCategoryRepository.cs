// Path: DNU.CanteenConnect.Web/Interfaces/IFoodCategoryRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho FoodCategoryRepository, kế thừa từ IRepository chung
    public interface IFoodCategoryRepository : IRepository<FoodCategory>
    {
        // Có thể thêm các phương thức đặc thù cho FoodCategory nếu cần
    }
}