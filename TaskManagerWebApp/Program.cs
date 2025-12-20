using Microsoft.EntityFrameworkCore;
using TaskManagerWebApp.Data;
using TaskManagerWebApp.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

// here we add DBContext with in-memory DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagerDb"));

var app = builder.Build();

// HTTP request pipeline
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

// seeding initial data for testing
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