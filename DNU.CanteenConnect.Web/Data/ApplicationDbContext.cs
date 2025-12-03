// DNU.CanteenConnect.Web/Data/ApplicationDbContext.cs
using DNU.CanteenConnect.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DNU.CanteenConnect.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Canteen> Canteens { get; set; } = default!;
        public DbSet<FoodCategory> FoodCategories { get; set; } = default!;
        public DbSet<FoodItem> FoodItems { get; set; } = default!;
        public DbSet<DailyMenu> DailyMenus { get; set; } = default!;
        public DbSet<MenuItem> MenuItems { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<LowStockAlert> LowStockAlerts { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // GIỮ NGUYÊN

            modelBuilder.Entity<Role>().Property(r => r.Id).ValueGeneratedNever(); // GIỮ NGUYÊN

            // --- ĐOẠN CODE MỚI DUY NHẤT ĐƯỢC THÊM VÀO ---
            // Cấu hình tường minh mối quan hệ giữa FoodItem và Review để sửa lỗi
            modelBuilder.Entity<FoodItem>()
                .HasMany(foodItem => foodItem.Reviews) // Một FoodItem có nhiều Review
                .WithOne(review => review.FoodItem) // Một Review thuộc về một FoodItem
                .HasForeignKey(review => review.ItemId); // Khóa ngoại trong bảng Review là cột 'ItemId'
            // ----------------------------------------------

            modelBuilder.Entity<FoodCategory>()
                .HasIndex(fc => fc.Name)
                .IsUnique(); // GIỮ NGUYÊN

            modelBuilder.Entity<DailyMenu>()
                .HasIndex(dm => new { dm.MenuDate, dm.CanteenId })
                .IsUnique(); // GIỮ NGUYÊN

            modelBuilder.Entity<FoodItem>()
                .HasOne(fi => fi.Canteen)
                .WithMany(c => c.FoodItems)
                .HasForeignKey(fi => fi.CanteenId)
                .OnDelete(DeleteBehavior.Restrict); // GIỮ NGUYÊN

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Canteen)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CanteenId)
                .OnDelete(DeleteBehavior.Restrict); // GIỮ NGUYÊN

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // GIỮ NGUYÊN

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade); // GIỮ NGUYÊN

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.FoodItem)
                .WithMany()
                .HasForeignKey(ci => ci.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict); // GIỮ NGUYÊN

            // --- NOTIFICATION CONFIGURATION ---
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Khi user bị xóa, notifications cũng bị xóa

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Order)
                .WithMany()
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Khi order bị xóa, notifications cũng bị xóa

            // Index for faster queries
            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.CreatedAt })
                .IsDescending(false, true); // UserId ascending, CreatedAt descending

            // --- LOW STOCK ALERT CONFIGURATION ---
            modelBuilder.Entity<LowStockAlert>()
                .HasOne(lsa => lsa.FoodItem)
                .WithMany()
                .HasForeignKey(lsa => lsa.FoodItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LowStockAlert>()
                .HasOne(lsa => lsa.Canteen)
                .WithMany()
                .HasForeignKey(lsa => lsa.CanteenId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for efficient alert queries
            modelBuilder.Entity<LowStockAlert>()
                .HasIndex(lsa => new { lsa.FoodItemId, lsa.CreatedAt })
                .IsDescending(false, true);

            modelBuilder.Entity<LowStockAlert>()
                .HasIndex(lsa => new { lsa.CanteenId, lsa.AlertStatus, lsa.CreatedAt })
                .IsDescending(false, false, true);

            // --- GIỮ NGUYÊN TOÀN BỘ PHẦN SEED DATA CỦA BẠN ---
            // Seed Canteen
            modelBuilder.Entity<Canteen>().HasData(
                new Canteen { CanteenId = 1, Name = "Nhà ăn C1", Location = "Tầng 1, Tòa nhà C", OpeningHours = "07:00 - 19:00" },
                new Canteen { CanteenId = 2, Name = "Nhà ăn B2", Location = "Tầng 2, Tòa nhà B", OpeningHours = "07:00 - 19:00" }
            );

            // Seed FoodCategory
            modelBuilder.Entity<FoodCategory>().HasData(
                new FoodCategory { CategoryId = 1, Name = "Món Chính", Description = "Các món ăn no" },
                new FoodCategory { CategoryId = 2, Name = "Món Phụ", Description = "Các món ăn kèm hoặc ăn vặt" },
                new FoodCategory { CategoryId = 3, Name = "Đồ Uống", Description = "Các loại nước giải khát" }
            );

            // Seed FoodItem
            modelBuilder.Entity<FoodItem>().HasData(
                new FoodItem { ItemId = 1, Name = "Cơm Gà Xối Mỡ", Description = "Cơm nóng hổi ăn kèm gà xối mỡ giòn tan", Price = 45000, ImageUrl = "https://topchuan.com/wp-content/uploads/2023/04/Com-Ga-Xoi-Mo-77-1.jpg", FoodCategoryCategoryId = 1, IsAvailable = true, IsSpecialOfTheDay = true, CanteenId = 1, StockQuantity = 50 },
                new FoodItem { ItemId = 2, Name = "Phở Bò", Description = "Phở truyền thống với thịt bò tươi", Price = 40000, ImageUrl = "https://cdn.tgdd.vn/Files/2017/03/18/962092/an-lien-3-bat-pho-voi-cong-thuc-nau-pho-nay-202201261419401397.jpg", FoodCategoryCategoryId = 1, IsAvailable = true, IsSpecialOfTheDay = false, CanteenId = 1, StockQuantity = 60 },
                new FoodItem { ItemId = 3, Name = "Trà Sữa Trân Châu Đường Đen", Description = "Trà sữa thơm ngon với trân châu đường đen dẻo dai", Price = 25000, ImageUrl = "https://cdn.tgdd.vn/Files/2022/01/21/1412109/huong-dan-cach-lam-tra-sua-tran-chau-duong-den-202201211522033706.jpg", FoodCategoryCategoryId = 3, IsAvailable = true, IsSpecialOfTheDay = true, CanteenId = 1, StockQuantity = 100 },
                new FoodItem { ItemId = 4, Name = "Bánh Mì Kẹp", Description = "Bánh mì giòn rụm kẹp thịt và rau tươi", Price = 20000, ImageUrl = "https://tapchiamthuc.net/wp-content/uploads/2023/03/banh-mi-kep-viet-nam-17.jpg", FoodCategoryCategoryId = 2, IsAvailable = true, IsSpecialOfTheDay = false, CanteenId = 1, StockQuantity = 80 },
                new FoodItem { ItemId = 5, Name = "Mì Ý Sốt Bò Băm", Description = "Mì Ý với sốt bò băm đậm đà", Price = 50000, ImageUrl = "https://cdn.tgdd.vn/Files/2019/04/15/1160777/cach-lam-mi-spaghetti-sot-bo-bam-trong-3-phut-voi-panzani-202203031421227202.jpg", FoodCategoryCategoryId = 1, IsAvailable = true, IsSpecialOfTheDay = false, CanteenId = 1, StockQuantity = 40 },
                new FoodItem { ItemId = 6, Name = "Nước Cam Ép", Description = "Nước cam ép tươi nguyên chất", Price = 18000, ImageUrl = "https://tse3.mm.bing.net/th?id=OIP.BMSD4FVGoFVZDnP6_0gYoQHaEk&pid=Api&P=0&h=180", FoodCategoryCategoryId = 3, IsAvailable = true, IsSpecialOfTheDay = false, CanteenId = 1, StockQuantity = 120 },
                new FoodItem { ItemId = 7, Name = "Nem Lụi", Description = "Nem lụi thơm ngon, ăn kèm nước chấm đặc biệt", Price = 35000, ImageUrl = "https://statics.vinpearl.com/nem-lui-nha-trang-07_1630987697.jpeg", FoodCategoryCategoryId = 2, IsAvailable = true, IsSpecialOfTheDay = false, CanteenId = 1, StockQuantity = 45 },
                new FoodItem { ItemId = 8, Name = "Bún Chả", Description = "Bún chả Hà Nội trứ danh", Price = 42000, ImageUrl = "https://tse2.mm.bing.net/th?id=OIP.0M4f-v1qaFU6jzyxobA9QAHaFj&pid=Api&P=0&h=180", FoodCategoryCategoryId = 1, IsAvailable = true, IsSpecialOfTheDay = false, CanteenId = 1, StockQuantity = 55 }
            );

            // Seed Role
            var adminRoleId = "a725130b-d248-4395-8178-01124e5251a1";
            var canteenStaffRoleId = "b845130b-d248-4395-8178-01124e5251a2";
            var customerRoleId = "c965130b-d248-4395-8178-01124e5251a3";

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new Role { Id = canteenStaffRoleId, Name = "CanteenStaff", NormalizedName = "CANTEENSTAFF" },
                new Role { Id = customerRoleId, Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Seed Admin User
            var adminUserId = "d125130b-d248-4395-8178-01124e5251a4";
            var hasher = new PasswordHasher<User>();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    UserName = "admin@canteen.com", NormalizedUserName = "ADMIN@CANTEEN.COM",
                    Email = "admin@canteen.com", NormalizedEmail = "ADMIN@CANTEEN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@123"),
                    SecurityStamp = "0a5a51c4-118d-4f11-9a74-9f20e4b868e4", ConcurrencyStamp = "f286828a-1a3b-4c4f-a719-7f51a4e21a2c",
                    CreatedDate = DateTime.UtcNow
                }
            );
            
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId }
            );
        }
    }
}