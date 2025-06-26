// DNU.CanteenConnect.Web/Program.cs

using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Cấu hình Services ---

// Cấu hình kết nối Database: Sử dụng SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)); // Đã thay đổi từ UseSqlServer sang UseSqlite

// Cấu hình Identity: Sử dụng ApplicationDbContext để lưu trữ Identity
builder.Services.AddDefaultIdentity<User>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false; // Đặt false cho dễ kiểm thử
    })
    .AddRoles<Role>() // Thêm hỗ trợ Role
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Thêm dịch vụ cho MVC (Controllers và Views)
builder.Services.AddControllersWithViews();

// --- Cấu hình HTTP Request Pipeline ---

var app = builder.Build();

// Cấu hình môi trường phát triển/sản xuất
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Sử dụng trang lỗi cho Migrations trong Development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Trang lỗi mặc định cho MVC trong Production
    app.UseHsts(); // Thêm HSTS trong Production
}

app.UseHttpsRedirection(); // Chuyển hướng HTTP sang HTTPS
app.UseStaticFiles();     // Cho phép phục vụ các tệp tĩnh (CSS, JS, hình ảnh)

app.UseRouting();         // Định tuyến các yêu cầu đến các endpoint phù hợp

app.UseAuthentication();  // Kích hoạt xác thực người dùng
app.UseAuthorization();   // Kích hoạt phân quyền người dùng

// Định nghĩa routing cho MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Tuyến đường mặc định: HomeController/Index

app.Run();