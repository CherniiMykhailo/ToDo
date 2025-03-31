using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.DTOs;

public class ToDoListDTO
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Please, enter a name of list.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<ToDo> ToDos { get; set; } = [];
}
