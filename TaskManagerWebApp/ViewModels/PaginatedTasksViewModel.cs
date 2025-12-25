using TaskManagerWebApp.Models;
using System.Collections.Generic;
using System;

namespace TaskManagerWebApp.ViewModels
{
    // used generic class to allow using it by other similar models in future
    public class PaginatedTasksViewModel<T>
    {
        // We will hold tasks for the current page only
        public List<T> Tasks { get; set; } = new List<T>();

        // Current page number (starts at 1)
        public int PageNumber { get; set; } = 1;

        // How many items per page
        public int PageSize { get; set; } = 5;

        // Total number of items (after filtering/searching)
        public int TotalItems { get; set; }

        // Total number of pages
        public int TotalPages { get; set; }

        // Current filter (all, active, completed)
        public string Filter { get; set; } = "all";

        // Current search string
        public string SearchString { get; set; } = "";

        // Helper properties (computed)
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // For showing "Showing 1-5 of 25 tasks" (pagenumber x pagesize) - (pagesize +1)
        public int FirstItemIndex => ((PageNumber - 1) * PageSize) + 1;
        public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalItems);

        // List of page numbers to display (e.g., [1, 2, 3, 4, 5]), will calculate it using helper method
        public List<int> PageNumbers { get; set; } = new List<int>();
    }
}