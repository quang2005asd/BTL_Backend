// Path: DNU.CanteenConnect.Web/Repositories/CanteenRepository.cs
using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Interfaces;
using DNU.CanteenConnect.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Repositories
{
    // Triển khai CanteenRepository
    public class CanteenRepository : Repository<Canteen>, ICanteenRepository
    {
        public CanteenRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Triển khai các phương thức đặc thù của ICanteenRepository nếu có
        // Hiện tại, không có phương thức đặc thù nào được định nghĩa trong ICanteenRepository,
        // nên nó chỉ thừa hưởng các phương thức từ Repository<Canteen>.
        // Nếu sau này có, bạn sẽ triển khai chúng ở đây.
    }
}