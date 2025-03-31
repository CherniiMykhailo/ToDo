using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public interface ITodoListDatabaseService
{
    Task<IEnumerable<ToDoList>> GetTodoListsAsync();
    Task<ToDoList> GetTodoListByIdAsync(int id);
    Task AddTodoListAsync(ToDoList todoList);
    Task UpdateTodoListAsync(ToDoList todoList);
    Task DeleteTodoListAsync(int id);
}
