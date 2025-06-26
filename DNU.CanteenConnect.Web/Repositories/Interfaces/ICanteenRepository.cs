// Path: DNU.CanteenConnect.Web/Interfaces/ICanteenRepository.cs
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Interfaces
{
    // Giao diện riêng cho CanteenRepository, kế thừa từ IRepository chung
    public interface ICanteenRepository : IRepository<Canteen>
    {
        // Có thể thêm các phương thức đặc thù cho Canteen nếu cần
        // Ví dụ: Task<IEnumerable<Canteen>> GetCanteensWithDailyMenusAsync();
    }
}