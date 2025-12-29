using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerWebApp.Models
{
    public class TaskItem
    {
        // Id is auto-incremented by the DB
        public int Id { get; set; }

        // We used these data annotations to validate Title attribute both in client-side and server-side
        [Required]
        [MaxLength(100)]

        public required string Title { get; set; }
        // since description is not required, make it nullable to bypass validation
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public Priority Priority { get; set; } = Priority.Medium;

    }

    public enum Priority
    {
        Low,
        High,
        Medium
    }

}