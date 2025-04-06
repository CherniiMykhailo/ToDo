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
        var toDoLists = await context.ToDoLists
            .Include(t => t.ToDos)
            .ToListAsync();
        return Ok(toDoLists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetToDoListById(int id)
    {
        var toDoList = await context.ToDoLists
            .Include(t => t.ToDos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (toDoList == null)
        {
            return NotFound();
        }

        return Ok(toDoList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateToDoList([FromBody] ToDoListDTO newList)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var exists = await context.ToDoLists.AnyAsync(t => t.Name == newList.Name);
        if (exists)
        {
            return BadRequest("A list with this name already exists.");
        }

        var toDoListEntity = new ToDoList
        {
            Id = newList.Id,
            Name = newList.Name
        };

        context.ToDoLists.Add(toDoListEntity);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllToDoLists), new { id = toDoListEntity.Id }, toDoListEntity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var toDoList = await context.ToDoLists.FindAsync(id);
        if (toDoList == null)
        {
            return NotFound();
        }

        context.ToDoLists.Remove(toDoList);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateToDoList(int id, [FromBody] ToDoList updatedList)
    {
        var existingList = await context.ToDoLists.FindAsync(id);
        if (existingList == null)
        {
            return NotFound();
        }

        existingList.Name = updatedList.Name;
        existingList.Description = updatedList.Description;

        await context.SaveChangesAsync();

        return Ok(existingList);
    }
}
