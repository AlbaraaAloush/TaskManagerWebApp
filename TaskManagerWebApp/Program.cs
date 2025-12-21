using Microsoft.EntityFrameworkCore;
using TaskManagerWebApp.Data;
using TaskManagerWebApp.Models;

/*
- Creates the application builder
-Loads:
    Configuration (appsettings.json)
    Environment variables
    Logging
    Dependency Injection container
 */
var builder = WebApplication.CreateBuilder(args);

// Enables controllers, Razor Pages, and Models
builder.Services.AddControllersWithViews();

// Add DBContext with in-memory DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagerDb"));

// Cannot add services after this line
var app = builder.Build();

// HTTP request Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Forces HTTPs
}

// Redirect to HTTPs automatically
app.UseHttpsRedirection();
// Enable CSS & JS
app.UseStaticFiles();
// Enable routing
app.UseRouting();
// Not used in my app
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize the DB with dummy data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    if (!context.TaskItems.Any())
    {
        context.TaskItems.AddRange(
            new TaskItem { Title = "Learn ASP.NET Core MVC", Description = "Complete the Task Manager project", CreatedDate = DateTime.Now.AddDays(-2) },
            new TaskItem { Title = "Breakfast with Colleagues", Description = "Milk, Eggs, Bread", IsCompleted = true, CreatedDate = DateTime.Now.AddDays(-1) },
            new TaskItem { Title = "Present Demo to Waddah", CreatedDate = DateTime.Now }
        );
        context.SaveChanges();
    }
}

app.Run();