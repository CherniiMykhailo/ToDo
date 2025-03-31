using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly ToDoListContext _context;

    public TodoListDatabaseService(ToDoListContext context)
    {
        this._context = context;
    }

    public async Task AddTodoListAsync(ToDoList todoList)
    {
        _ = this._context.ToDoLists.Add(todoList);
        await this._context.SaveChangesAsync();
    }

    public async Task DeleteTodoListAsync(int id)
    {
        var entity = await _context.ToDoLists.FindAsync(id);

        if (entity == null)
        {
            throw new Exception("TodoList not found");
        }

        this._context.ToDoLists.Remove(entity);
        await this._context.SaveChangesAsync();
    }

    public async Task<ToDoList> GetTodoListByIdAsync(int id)
    {
        var entity = await this._context.ToDoLists.Include(t => t.ToDos).FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null)
        {
            throw new Exception("TodoList not found");
        }
        return entity;
    }

    public async Task<IEnumerable<ToDoList>> GetTodoListsAsync()
    {
        return await this._context.ToDoLists
            .Include(t => t.ToDos).ToListAsync();
    }

    public async Task UpdateTodoListAsync(ToDoList todoList)
    {
        var entity = await _context.ToDoLists.Include(t => t.ToDos).FirstOrDefaultAsync(t => t.Id == todoList.Id);
        if (entity == null)
        {
            throw new Exception("TodoList not found");
        }

        entity.Name = todoList.Name;
        entity.Description = todoList.Description;
        entity.ToDos = todoList.ToDos;

        await this._context.SaveChangesAsync();
    }
}
