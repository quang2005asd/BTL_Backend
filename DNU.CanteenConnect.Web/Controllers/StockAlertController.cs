using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using DNU.CanteenConnect.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DNU.CanteenConnect.Web.Controllers
{
    [Authorize(Roles = "Admin,CanteenStaff")]
    public class StockAlertController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockAlertService _alertService;
        private const int PageSize = 20;

        public StockAlertController(ApplicationDbContext context, IStockAlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        // GET: StockAlert
        public async Task<IActionResult> Index(int page = 1)
        {
            // Get user's canteen if staff, otherwise show all
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var canteenId = 1; // Default to first canteen

            // If user is canteen staff, get their canteen
            if (userRole == "CanteenStaff")
            {
                // For demo, assuming canteen ID 1
                canteenId = 1;
            }

            // Get active alerts
            var activeAlerts = await _alertService.GetActiveAlertsAsync(canteenId);
            var criticalItems = await _alertService.GetCriticalItemsAsync(canteenId);

            // Get all alerts with pagination
            var totalAlerts = await _context.LowStockAlerts
                .Where(a => a.CanteenId == canteenId)
                .CountAsync();

            var alerts = await _context.LowStockAlerts
                .Where(a => a.CanteenId == canteenId)
                .Include(a => a.FoodItem)
                .Include(a => a.Canteen)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalAlerts / (double)PageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalAlerts = totalAlerts;
            ViewBag.ActiveCount = activeAlerts.Count;
            ViewBag.CriticalCount = criticalItems.Count;
            ViewBag.CanteenId = canteenId;
            ViewBag.CriticalItems = criticalItems;

            return View(alerts);
        }

        // POST: StockAlert/MarkResolved
        [HttpPost]
        public async Task<IActionResult> MarkResolved(int id)
        {
            try
            {
                await _alertService.MarkAlertResolvedAsync(id);
                return Json(new { success = true, message = "Alert marked as resolved" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: StockAlert/GetCriticalItems
        [HttpGet]
        public async Task<IActionResult> GetCriticalItems(int canteenId = 1)
        {
            var criticalItems = await _alertService.GetCriticalItemsAsync(canteenId);
            return Json(new
            {
                count = criticalItems.Count,
                items = criticalItems.Select(a => new
                {
                    a.AlertId,
                    itemName = a.FoodItem?.Name,
                    a.CurrentStock,
                    a.ThresholdStock,
                    a.CreatedAt
                }).ToList()
            });
        }

        // GET: StockAlert/GetAlerts
        [HttpGet]
        public async Task<IActionResult> GetAlerts(int canteenId = 1)
        {
            var alerts = await _alertService.GetActiveAlertsAsync(canteenId);
            return Json(new
            {
                count = alerts.Count,
                alerts = alerts.Select(a => new
                {
                    a.AlertId,
                    itemName = a.FoodItem?.Name,
                    a.CurrentStock,
                    status = a.AlertStatus,
                    createdAt = a.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    notes = a.Notes
                }).ToList()
            });
        }

        // GET: StockAlert/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var canteenId = 1;
            var activeAlerts = await _alertService.GetActiveAlertsAsync(canteenId);
            var criticalItems = await _alertService.GetCriticalItemsAsync(canteenId);

            var model = new
            {
                TotalAlerts = await _context.LowStockAlerts.CountAsync(a => a.CanteenId == canteenId),
                ActiveAlerts = activeAlerts.Count,
                CriticalItems = criticalItems.Count,
                RecentAlerts = activeAlerts.Take(10).ToList(),
                CriticalItemsList = criticalItems.ToList()
            };

            return View("Dashboard", model);
        }
    }
}
