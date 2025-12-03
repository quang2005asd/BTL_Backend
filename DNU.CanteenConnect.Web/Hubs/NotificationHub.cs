using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Hubs
{
    public class NotificationHub : Hub
    {
        // Phương thức này cho phép server gửi thông báo đến một người dùng cụ thể
        public async Task SendNotificationToUser(string userId, string message)
        {
            // Clients.User(userId) sẽ chỉ gửi đến client nào có ID người dùng khớp
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
