using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext with in-memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagerDb"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
        context.TaskItems.AddRange(
            new TaskItem { Title = "Learn ASP.NET Core MVC", Description = "Complete the Task Manager project", CreatedDate = DateTime.Now.AddDays(-2) },
            new TaskItem { Title = "Buy groceries", Description = "Milk, Eggs, Bread", IsCompleted = true, CreatedDate = DateTime.Now.AddDays(-1) },
            new TaskItem { Title = "Schedule dentist appointment", CreatedDate = DateTime.Now }
        );
        context.SaveChanges();
    }
}

app.Run();