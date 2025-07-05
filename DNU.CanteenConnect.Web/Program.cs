using DNU.CanteenConnect.Web.Data;
using DNU.CanteenConnect.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// ========== 1. CẤU HÌNH DATABASE VÀ IDENTITY ==========
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// ========== 2. THÊM SESSION ==========
builder.Services.AddDistributedMemoryCache(); // Bộ nhớ tạm
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========== 3. DỊCH VỤ MVC / RAZOR ==========
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ========== 4. BUILD APP ==========
var app = builder.Build();

// ========== 5. HTTP REQUEST PIPELINE ==========
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

// *** QUAN TRỌNG: Session phải nằm trước UseAuthentication ***
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();