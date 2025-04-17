using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.DTOs;

namespace TodoListApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoListController : Controller
{
    private readonly ToDoListContext context;

    public TodoListController(ToDoListContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllToDoLists()
    {
        var toDoLists = await this.context.ToDoLists
            .Include(t => t.ToDos)
            .ToListAsync();
        return this.Ok(toDoLists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetToDoListById(int id)
    {
        var toDoList = await this.context.ToDoLists
            .Include(t => t.ToDos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (toDoList == null)
        {
            return this.NotFound();
        }

        return this.Ok(toDoList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateToDoList([FromBody] ToDoListDTO newList)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var exists = await this.context.ToDoLists.AnyAsync(t => t.Name == newList.Name);
        if (exists)
        {
            return this.BadRequest("A list with this name already exists.");
        }

        var toDoListEntity = new ToDoList
        {
            Id = newList.Id,
            Name = newList.Name
        };

        _ = this.context.ToDoLists.Add(toDoListEntity);
        _ = await this.context.SaveChangesAsync();

        return this.CreatedAtAction(nameof(GetAllToDoLists), new { id = toDoListEntity.Id }, toDoListEntity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var toDoList = await this.context.ToDoLists.FindAsync(id);
        if (toDoList == null)
        {
            return this.NotFound();
        }

        _ = this.context.ToDoLists.Remove(toDoList);
        _ = await this.context.SaveChangesAsync();

        return this.NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateToDoList(int id, [FromBody] ToDoList updatedList)
    {
        var existingList = await this.context.ToDoLists.FindAsync(id);
        if (existingList == null)
        {
            return this.NotFound();
        }

        existingList.Name = updatedList.Name;
        existingList.Description = updatedList.Description;

        _ = await this.context.SaveChangesAsync();

        return this.Ok(existingList);
    }
}
