using Microsoft.EntityFrameworkCore;
using TaskManagerWebApp.Models;

namespace TaskManagerWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        // database declaration and table name 
        public DbSet<TaskItem> TaskItems { get; set; }
    }
}