using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerWebApp.Data;
using TaskManagerWebApp.Models;
using TaskManagerWebApp.ViewModels;

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
        public async Task<IActionResult> Index(string filter = "all", string searchString = "", int pageNumber = 1, int pageSize = 5)
        {
            // Prevent user from inputting pagenumber less than 1
            if (pageNumber < 1) pageNumber = 1;

            // Build the query (deferred execution)
            IQueryable<TaskItem> tasksQuery = context.TaskItems;

            // Reduce the dataset first then apply pagination
            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                string caseInsensitiveSearch = searchString.ToUpper();
                tasksQuery = tasksQuery.Where(t =>
                    t.Title.ToUpper().Contains(caseInsensitiveSearch) ||
                    (t.Description != null && t.Description.ToUpper().Contains(caseInsensitiveSearch)));
            }

            // Apply completion filter
            switch (filter.ToLower())
            {
                case "active":
                    tasksQuery = tasksQuery.Where(t => !t.IsCompleted);
                    break;
                case "completed":
                    tasksQuery = tasksQuery.Where(t => t.IsCompleted);
                    break;
                    // "all" shows everything
            }

            // Get total count BEFORE pagination
            int totalItems = await tasksQuery.CountAsync();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Ensure page number is valid
            if (pageNumber > totalPages && totalPages > 0)
                pageNumber = totalPages;

            // Apply pagination (skip and take)
            var pagedTasks = await tasksQuery
                .OrderByDescending(t => t.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Generate page numbers for display (e.g., [1, 2, 3, 4, 5])
            var pageNumbers = GeneratePageNumbers(pageNumber, totalPages);

            // Create and populate ViewModel
            var viewModel = new PaginatedTasksViewModel<TaskItem>
            {
                Data = pagedTasks,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Filter = filter,
                SearchString = searchString,
                PageNumbers = pageNumbers
            };

            return View(viewModel);
        }

        // Helper method to generate page numbers
        private List<int> GeneratePageNumbers(int currentPage, int totalPages, int maxPagesToShow = 5)
        {
            var pageNumbers = new List<int>();

            if (totalPages <= maxPagesToShow)
            {
                // Show all pages if we have 5 or fewer
                for (int i = 1; i <= totalPages; i++)
                    pageNumbers.Add(i);
            }
            else
            {
                // Show a window of pages around current page
                int startPage = Math.Max(1, currentPage - (maxPagesToShow / 2));
                int endPage = Math.Min(totalPages, startPage + maxPagesToShow - 1);

                // Adjust if we're near the beginning or end
                if (endPage - startPage + 1 < maxPagesToShow)
                {
                    startPage = Math.Max(1, endPage - maxPagesToShow + 1);
                }

                for (int i = startPage; i <= endPage; i++)
                    pageNumbers.Add(i);
            }

            return pageNumbers;
        }

        // GET: TaskItems/Upsert
        public async Task<IActionResult> Form(int? id)
        {
            if(id is null || id == 0)
            {
                return View("Form", new TaskItem { Title = "" });
            }
            else
            {
                // use FindAsync since you're looking for single item by its primary key
                var taskItem = await context.TaskItems.FindAsync(id);

                if (taskItem == null)
                    return NotFound();

                return View(taskItem);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                if(taskItem.Id == 0)
                {
                    taskItem.CreatedDate = DateTime.Now;
                    context.Add(taskItem);
                    await context.SaveChangesAsync();
                    // we use redirect to prevent duplicate submission when refreshing the page
                    return RedirectToAction("Index");
                }
                else
                {
                    context.Update(taskItem);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            return View(taskItem);
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
        public async Task<IActionResult> DeleteConfirmed(int id, string currentFilter, string currentSearchString, int currentPageNumber,int currentPageSize)
        {
            var taskItem = await context.TaskItems.FindAsync(id);
            if (taskItem != null)
            {
                context.TaskItems.Remove(taskItem);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new
            {
                Filter = currentFilter,
                SearchString = currentSearchString,
                PageNumber = currentPageNumber,
                PageSize = currentPageSize
            });
        }

    }
}