using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TodoListApp.WebApp.Models;

public class ToDo
{
    public int ToDoId { get; set; }

    [Required(ErrorMessage = "Please enter a description.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a due time.")]
    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Please enter a category.")]
    public string CategoryId { get; set; } = string.Empty;

    [ValidateNever]
    public Category Category { get; set; } = null!;

    [Required(ErrorMessage = "Please enter a status.")]
    public string StatusId { get; set; } = string.Empty;

    [ValidateNever]
    public Status Status { get; set; } = null!;

    public bool Overdue => this.StatusId == "open" && this.DueDate < DateTime.Today;
}
