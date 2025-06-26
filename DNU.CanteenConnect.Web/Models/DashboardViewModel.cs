using System.Collections.Generic;

namespace DNU.CanteenConnect.Web.Models
{
    public class DashboardViewModel
    {
        // Thống kê trong ngày
        public decimal RevenueToday { get; set; }
        public int NewOrdersToday { get; set; }
        public int PendingOrders { get; set; }

        // Thống kê 30 ngày
        public decimal RevenueLast30Days { get; set; }
        public List<DailyRevenueViewModel> Last30DaysRevenueChartData { get; set; }

        // Thống kê khác
        public int TotalUsers { get; set; }
        public List<TopSellingItemViewModel> TopSellingFoodItems { get; set; }
    }
}