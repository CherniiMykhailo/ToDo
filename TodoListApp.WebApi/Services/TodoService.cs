using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.DTOs;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public class TodoService : ITodoService
{
    private readonly ToDoListContext _context;

    public TodoService(ToDoListContext context)
    {
        this._context = context;
    }

    public async Task AddTodoAsync(ToDo todo)
    {
        this._context.ToDos.Add(todo);
        await _context.SaveChangesAsync();
    }

    public ToDo ConvertToEntity(ToDoDTO dto)
    {
        return new ToDo
        {
            Description = dto.Description,
            DueDate = dto.DueDate,
            CategoryId = dto.CategoryId,
            StatusId = dto.StatusId,
            ToDoListId = dto.ToDoListId
        };
    }

    public async Task DeleteTodoAsync(int id)
    {
        var entity = await _context.ToDos.FindAsync(id);

        if (entity == null)
        {
            throw new Exception("Todo not found");
        }

        this._context.ToDos.Remove(entity);
        await this._context.SaveChangesAsync();
    }

    public Task<IEnumerable<ToDo>> GetTodoAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ToDo> GetTodoByIdAsync(int id)
    {
        var entity = await this._context.ToDos.FirstOrDefaultAsync(t => t.ToDoId == id);

        if (entity == null)
        {
            throw new Exception("TodoList not found");
        }

        return entity;
    }

    public Task UpdateTodoAsync(ToDo todo)
    {
        throw new NotImplementedException();
    }
}
