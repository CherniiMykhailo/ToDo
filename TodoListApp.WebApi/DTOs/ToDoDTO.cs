using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TodoListApp.WebApi.DTOs;
public class ToDoDTO
{
    public int ToDoId { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string CategoryId { get; set; }
    public string StatusId { get; set; }
    public int? ToDoListId { get; set; }
    public bool Overdue { get; set; }
    public string? CreatedBy { get; set; }
    public string? AssignedTo { get; set; }
}
