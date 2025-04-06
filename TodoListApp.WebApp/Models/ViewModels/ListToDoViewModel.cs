namespace TodoListApp.WebApp.Models.ViewModels;

public class ListToDoViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<ToDoViewModel>? ToDos { get; set; }
}
