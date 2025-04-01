using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ToDoViewModel
{
    [Key]
    public int ToDoId { get; set; }

    [Required(ErrorMessage = "Please enter a description.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a due time.")]
    public DateTime? DueDate { get; set; }

    public bool Overdue => this.DueDate < DateTime.Today;
}
