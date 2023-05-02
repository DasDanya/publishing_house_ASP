using Microsoft.EntityFrameworkCore;
using publishing.Models;
using Microsoft.AspNetCore.Identity;
using publishing.Data;
using publishing.Areas.Identity.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddDbContext<PublishingDBContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("PublishingDB")));

//builder.Services.AddDefaultIdentity<publishingUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<publishingContext>();

builder.Services.AddDbContext<publishingContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("PublishingDB")));

// ��� �������
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<publishingUser, IdentityRole>(options => 
options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<publishingContext>();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
    opt.LoginPath = new PathString("/Identity/Account/Login");
});

var app = builder.Build();

// ��� �������
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "customers",
    pattern: "/customers/{typeProduct?}",
    defaults: new { controller = "Customers", action = "SelectProducts" });

app.MapRazorPages();
app.Run();
