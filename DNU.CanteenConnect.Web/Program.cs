using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

// ========== CẤU HÌNH PORT CHO RENDER (GIỮ NGUYÊN) ==========
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://0.0.0.0:{port}");

var builder = WebApplication.CreateBuilder(args);

// ========== 1. CẤU HÌNH DATABASE VÀ IDENTITY ==========
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Cấu hình kết nối PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Cấu hình Identity (User, Role)
builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// ========== 2. THÊM SESSION ==========
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========== 3. DỊCH VỤ MVC / RAZOR ==========
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ========== 4. BUILD APP ==========
var app = builder.Build();

// ========== 5. TỰ ĐỘNG CHẠY MIGRATION (PHẦN QUAN TRỌNG MỚI THÊM) ==========
// Đoạn này giúp tạo bảng tự động trên Render khi web khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Tự động tạo bảng nếu chưa có
        Console.WriteLine("--> Đã chạy Migration (Tạo bảng) thành công!");
    }
    catch (Exception ex)
    {
        // Ghi log lỗi nếu không tạo được bảng
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "--> Lỗi nghiêm trọng khi chạy Migration.");
    }
}

// ========== 6. HTTP REQUEST PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Kích hoạt Session

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();