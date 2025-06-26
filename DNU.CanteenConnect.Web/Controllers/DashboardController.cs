using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var thirtyDaysAgo = today.AddDays(-30);

            // --- Thống kê trong ngày ---
            var revenueToday = await _context.Orders
                .Where(o => o.Status == "Completed" && o.OrderDate >= today && o.OrderDate < tomorrow)
                .SumAsync(o => o.TotalAmount);

            var newOrdersToday = await _context.Orders
                .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
                .CountAsync();
            
            // --- Thống kê tổng thể & 30 ngày ---
            var pendingStatuses = new List<string> { "Pending", "AwaitingPaymentConfirmation", "PaymentSubmitted", "Paid", "Preparing" };
            var pendingOrders = await _context.Orders
                .Where(o => pendingStatuses.Contains(o.Status))
                .CountAsync();

            var totalUsers = await _context.Users.CountAsync();

            var last30DaysOrders = await _context.Orders
                .Where(o => o.Status == "Completed" && o.OrderDate >= thirtyDaysAgo)
                .ToListAsync();

            var revenueLast30Days = last30DaysOrders.Sum(o => o.TotalAmount);
            
            var last30DaysRevenueChartData = last30DaysOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailyRevenueViewModel
                {
                    Date = g.Key.ToString("dd/MM"),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            var topSellingFoodItems = await _context.OrderItems
                .Where(oi => oi.Order!.OrderDate >= thirtyDaysAgo && (oi.Order.Status == "Completed" || oi.Order.Status == "Paid"))
                .GroupBy(oi => oi.FoodItem!.Name)
                .Select(g => new TopSellingItemViewModel { FoodName = g.Key, TotalQuantitySold = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.TotalQuantitySold).Take(5).ToListAsync();
            
            var viewModel = new DashboardViewModel
            {
                RevenueToday = revenueToday,
                NewOrdersToday = newOrdersToday,
                PendingOrders = pendingOrders,
                RevenueLast30Days = revenueLast30Days,
                Last30DaysRevenueChartData = last30DaysRevenueChartData,
                TotalUsers = totalUsers,
                TopSellingFoodItems = topSellingFoodItems
            };

            return View(viewModel);
        }
    }
}