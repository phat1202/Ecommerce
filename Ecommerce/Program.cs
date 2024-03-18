using Ecommerce.Extensions;
using Ecommerce.Helpers;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidCastException("Not");
builder.Services.AddDbContext<EcommerceDbContext>(options => options.UseMySQL(connectionString));

builder.Services.AddAutoMapper(typeof(MappingClass));
builder.Services.AddTransient<StatsService>();
builder.Services.AddTransient<EmailSender>();
builder.Services.AddSingleton<ImageUpLoading>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
    options.AccessDeniedPath = "/User/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(2);
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute(
                     "/Home",
                     "/ChiTietSanPham.html"
                 );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
