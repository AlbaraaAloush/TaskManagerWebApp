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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("TaskManagerDb")
           .LogTo(Console.WriteLine, LogLevel.Information); 
});

// Cannot add services after this line
var app = builder.Build();

// HTTP request Middleware pipeline for handling exceptions
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
// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Clear existing data
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    // Add sample tasks
    if (!context.TaskItems.Any())
    {
        var tasks = new List<TaskItem>();

        // Create 25 sample tasks
        for (int i = 1; i <= 25; i++)
        {
            tasks.Add(new TaskItem
            {
                Title = $"Task {i}: Learn ASP.NET Core feature {i}",
                Description = i % 2 == 0 ? $"Detailed description for task {i}" : null,
                IsCompleted = i % 3 == 0, // Every 3rd task completed
                CreatedDate = DateTime.Now.AddDays(-i),
                Priority = (Priority)(i % 3) // Cycle through Low, High, Medium
            });
        }

        context.TaskItems.AddRange(tasks);
        context.SaveChanges();
    }
}

app.Run();