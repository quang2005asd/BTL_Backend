// DNU.CanteenConnect.Web/Models/Menu.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DNU.CanteenConnect.Web.Models
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime MenuDate { get; set; }

        public int CanteenId { get; set; }
        public virtual Canteen Canteen { get; set; }

        public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}