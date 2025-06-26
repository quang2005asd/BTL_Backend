using DNU.CanteenConnect.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Models
{
    public class OrderManagementViewModel
    {
        public PaginatedList<Order> Orders { get; set; }
        public SelectList StatusFilterOptions { get; set; }
        public string CurrentStatusFilter { get; set; }
        public string CurrentSearchString { get; set; }
    }
}