// DNU.CanteenConnect.Web/Models/FoodItemManagementViewModel.cs
using DNU.CanteenConnect.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNU.CanteenConnect.Web.Models
{
    public class FoodItemManagementViewModel
    {
        public PaginatedList<FoodItem> FoodItems { get; set; }
        public SelectList Canteens { get; set; }
        public SelectList Categories { get; set; }
        public string? SearchString { get; set; }
        public int? CanteenId { get; set; }
        public int? CategoryId { get; set; }
    }
}