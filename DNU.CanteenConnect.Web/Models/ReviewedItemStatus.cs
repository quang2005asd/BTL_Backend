namespace DNU.CanteenConnect.Web.Models
{
    public class ReviewedItemStatus
    {
        public OrderItem OrderItem { get; set; }
        public bool HasBeenReviewed { get; set; }
    }
}