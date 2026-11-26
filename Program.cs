using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";
    var role = context.Session.GetString("Role");

    bool blocked =
        (path.StartsWith("/admin") && role != "Admin") ||
        (path.StartsWith("/staff") && role != "Staff" && role != "Admin") ||
        (path.StartsWith("/customer") && role != "Customer");

    if (blocked)
    {
        // Not logged in → login page. Logged in but wrong role → access denied.
        context.Response.Redirect(role == null ? "/Account/Login" : "/Account/AccessDenied");
        return;
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
