using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TodoListApp.WebApi;


namespace TodoListApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToDoController : Controller
{
    private readonly ToDoListContext context;

    public ToDoController(ToDoListContext context)
    {
        this.context = context;
    }

    [HttpGet("{listId}/tasks")]
    public async Task<IActionResult> GetTasksByListId(int listId)
    {
        var tasks = await context.ToDos.Where(t => t.ToDoListId == listId).ToListAsync();
        return Ok(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> GetTask()
    {
        var toDo = await context.ToDos.ToListAsync();
        return Ok(toDo);
    }


    [HttpGet("tasks/{id}")]
    public async Task<IActionResult> GetTaskDetails(int id)
    {
        var task = await context.ToDos
            .Include(t => t.Category)
            .Include(t => t.Status)
            .FirstOrDefaultAsync(t => t.ToDoId == id);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ToDoDTO toDo)
    {
        try
        {
            var existingStatus = await context.Statuses
                .FirstOrDefaultAsync(s => s.StatusId == toDo.StatusId);
            if (existingStatus == null)
            {
                return BadRequest("Bad Status.");
            }

            var existingCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == toDo.CategoryId);
            if (existingCategory == null)
            {
                return BadRequest("Bad category.");
            }

            if (ModelState.IsValid)
            {
                var toDoEntity = new ToDo
                {
                    Description = toDo.Description,
                    DueDate = toDo.DueDate,
                    StatusId = toDo.StatusId,
                    CategoryId = toDo.CategoryId,
                    ToDoListId = toDo.ToDoListId
                };

                this.context.ToDos.Add(toDoEntity);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTaskDetails), new { id = toDoEntity.ToDoId }, toDoEntity);
            }

            return BadRequest(ModelState);
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "not add new todo.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var toDo = await context.ToDos.FindAsync(id);

        if (toDo == null)
        {
            return NotFound("Завдання з таким ID не знайдено.");
        }

        context.ToDos.Remove(toDo);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ToDo updatedToDo)
    {
        var existingToDo = await context.ToDos
            .Include(t => t.Category)
            .Include(t => t.Status)
            .FirstOrDefaultAsync(t => t.ToDoId == id);

        if (existingToDo == null)
        {
            return NotFound("Завдання з таким ID не знайдено.");
        }

        var existingCategory = await context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == updatedToDo.CategoryId);
        if (existingCategory == null)
        {
            return BadRequest("Невірна категорія.");
        }

        var existingStatus = await context.Statuses
            .FirstOrDefaultAsync(s => s.StatusId == updatedToDo.StatusId);
        if (existingStatus == null)
        {
            return BadRequest("Невірний статус.");
        }

        existingToDo.Description = updatedToDo.Description;
        existingToDo.DueDate = updatedToDo.DueDate;
        existingToDo.CategoryId = updatedToDo.CategoryId;
        existingToDo.StatusId = updatedToDo.StatusId;
        existingToDo.Category = existingCategory;
        existingToDo.Status = existingStatus;

        await context.SaveChangesAsync();

        return Ok(existingToDo);
    }

    [HttpPut("complete/{id}")]
    public async Task<IActionResult> MarkComplete(int id)
    {
        var task = await context.ToDos.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        task.StatusId = "completed";
        await context.SaveChangesAsync();
        return NoContent();
    }
}
