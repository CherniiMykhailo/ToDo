using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ToDoViewModel
{
    public int ToDoId { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public bool Overdue { get; set; }
}
