using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerWebApp.Data;
using TaskManagerWebApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagerWebApp.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext context;

        // here we're injecting dependency through constructor
        public TaskItemsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // GET: TaskItems

        // default filter value is all
        public async Task<IActionResult> Index(string filter = "all")
        {
            // saves the currently selected filter
            ViewBag.CurrentFilter = filter;
            // deferred execution
            IQueryable<TaskItem> tasks = context.TaskItems;
            if(filter == "active")
            {
                tasks = tasks.Where(t => !t.IsCompleted);
            } else if(filter == "completed")
            {
                tasks = tasks.Where(t => t.IsCompleted);
            }
             
            // we use async to avoid blocking main thread and let the task execute in background
           var taskList = await tasks.OrderByDescending(t => t.CreatedDate).ToListAsync();
            return View(taskList);
        }

        // GET: TaskItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(TaskItem taskItem)
        {
            // model instantiaion and fields assignement is done automatically by ASP.NET

            if (ModelState.IsValid)
            {
                taskItem.CreatedDate = DateTime.Now;
                context.Add(taskItem);
                await context.SaveChangesAsync();
                // we use redirect to prevent duplicate submission when refreshing the page
                return RedirectToAction("Index");
            }

            return View(taskItem);
        
        }

        // GET: TaskItems/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // use FindAsync since you're looking for single item by its primary key
            var taskItem = await context.TaskItems.FindAsync(id);
            return View(taskItem);
        }

        // POST: TaskItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskItem taskItem)
        {
                context.Update(taskItem);
                await context.SaveChangesAsync();
               
                return RedirectToAction("Index");
           
        }

        // POST: TaskItems/ToggleComplete/5
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var taskItem = await context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.IsCompleted = !taskItem.IsCompleted;
            context.Update(taskItem);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: TaskItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await context.TaskItems.FindAsync(id);
            if (taskItem != null)
            {
                context.TaskItems.Remove(taskItem);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}