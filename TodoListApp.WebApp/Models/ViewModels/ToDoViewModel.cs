using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ToDoViewModel
{
    public int ToDoId { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public string CategoryId { get; set; }
    public string StatusId { get; set; }
    public int? TodoListId { get; set; }
    public string? CreatedBy { get; set; }
    public bool Overdue { get; set; }
}
