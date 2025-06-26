using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Models
{
    public class OrderHistoryDetailViewModel
    {
        public Order Order { get; set; }
        public List<ReviewedItemStatus> ItemReviewStatuses { get; set; } = new List<ReviewedItemStatus>();
    }
}