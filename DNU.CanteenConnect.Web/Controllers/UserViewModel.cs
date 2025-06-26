using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Models
{
    public class UserViewModel
    {
        public User User { get; set; }
        public IList<string> Roles { get; set; }
    }
}