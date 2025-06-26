using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // Cần cho GetRequiredService

namespace DNU.CanteenConnect.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            // Lấy RoleManager từ serviceProvider
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            // Định nghĩa các vai trò cần tạo
            string[] roleNames = { "Admin", "Staff", "Customer" };

            foreach (var roleName in roleNames)
            {
                // Kiểm tra xem vai trò đã tồn tại chưa
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Nếu chưa tồn tại, tạo vai trò mới
                    // Role Id sẽ được Guid.NewGuid().ToString() tạo tự động trong CreateAsync
                    await roleManager.CreateAsync(new Role { Name = roleName, NormalizedName = roleName.ToUpper() });
                }
            }
        }
    }
}