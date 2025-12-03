using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DNU.CanteenConnect.Web.Services
{
    public interface IStockAlertService
    {
        Task CheckAndCreateAlertAsync(int foodItemId, int canteenId, int currentStock);
        Task<List<LowStockAlert>> GetActiveAlertsAsync(int canteenId);
        Task<List<LowStockAlert>> GetCriticalItemsAsync(int canteenId);
        Task MarkAlertResolvedAsync(int alertId);
    }

    public class StockAlertService : IStockAlertService
    {
        private readonly ApplicationDbContext _context;
        private const int DEFAULT_THRESHOLD = 5;
        private const int CRITICAL_THRESHOLD = 3;

        public StockAlertService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if stock is low and creates or updates alert accordingly
        /// </summary>
        public async Task CheckAndCreateAlertAsync(int foodItemId, int canteenId, int currentStock)
        {
            try
            {
                var foodItem = await _context.FoodItems.FindAsync(foodItemId);
                if (foodItem == null) return;

                // Check if stock is below threshold
                if (currentStock < DEFAULT_THRESHOLD)
                {
                    // Check if an active alert already exists for this item
                    var existingAlert = await _context.LowStockAlerts
                        .FirstOrDefaultAsync(a => a.FoodItemId == foodItemId 
                            && a.CanteenId == canteenId 
                            && a.AlertStatus == "Active");

                    if (existingAlert != null)
                    {
                        // Update existing alert with current stock
                        existingAlert.CurrentStock = currentStock;
                        existingAlert.CreatedAt = DateTime.Now;
                    }
                    else
                    {
                        // Create new alert
                        var newAlert = new LowStockAlert
                        {
                            FoodItemId = foodItemId,
                            CanteenId = canteenId,
                            CurrentStock = currentStock,
                            ThresholdStock = DEFAULT_THRESHOLD,
                            AlertStatus = "Active",
                            CreatedAt = DateTime.Now,
                            Notes = $"Sản phẩm {foodItem.Name} có số lượng thấp ({currentStock}/{DEFAULT_THRESHOLD})"
                        };

                        _context.LowStockAlerts.Add(newAlert);
                    }

                    await _context.SaveChangesAsync();
                }
                else if (currentStock >= DEFAULT_THRESHOLD)
                {
                    // Stock is back to normal, mark alert as resolved
                    var activeAlert = await _context.LowStockAlerts
                        .FirstOrDefaultAsync(a => a.FoodItemId == foodItemId 
                            && a.CanteenId == canteenId 
                            && a.AlertStatus == "Active");

                    if (activeAlert != null)
                    {
                        activeAlert.AlertStatus = "Resolved";
                        activeAlert.ResolvedAt = DateTime.Now;
                        activeAlert.Notes = $"Hàng được bổ sung. Số lượng hiện tại: {currentStock}";
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw - stock decrement is more important than alert creation
                System.Diagnostics.Debug.WriteLine($"Error in StockAlertService: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all active alerts for a canteen, ordered by creation date
        /// </summary>
        public async Task<List<LowStockAlert>> GetActiveAlertsAsync(int canteenId)
        {
            return await _context.LowStockAlerts
                .Where(a => a.CanteenId == canteenId && a.AlertStatus == "Active")
                .Include(a => a.FoodItem)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets critical items (stock < 3) for a canteen
        /// </summary>
        public async Task<List<LowStockAlert>> GetCriticalItemsAsync(int canteenId)
        {
            return await _context.LowStockAlerts
                .Where(a => a.CanteenId == canteenId 
                    && a.AlertStatus == "Active" 
                    && a.CurrentStock < CRITICAL_THRESHOLD)
                .Include(a => a.FoodItem)
                .OrderBy(a => a.CurrentStock) // Show lowest first
                .ToListAsync();
        }

        /// <summary>
        /// Manually resolve an alert
        /// </summary>
        public async Task MarkAlertResolvedAsync(int alertId)
        {
            var alert = await _context.LowStockAlerts.FindAsync(alertId);
            if (alert != null)
            {
                alert.AlertStatus = "Resolved";
                alert.ResolvedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
