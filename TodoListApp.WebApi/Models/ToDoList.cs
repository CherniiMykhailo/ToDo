using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;

public class ToDoList
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Please, enter a name of list.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<ToDo> ToDos { get; set; } = [];
}
