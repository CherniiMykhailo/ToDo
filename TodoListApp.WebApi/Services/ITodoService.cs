using TodoListApp.WebApi.DTOs;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public interface ITodoService
{
    ToDo ConvertToEntity(ToDoDTO dto);
    Task<IEnumerable<ToDo>> GetTodoAsync();
    Task<ToDo> GetTodoByIdAsync(int id);
    Task AddTodoAsync(ToDo todo);
    Task UpdateTodoAsync(ToDo todo);
    Task DeleteTodoAsync(int id);
}
