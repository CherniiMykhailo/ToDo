using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.DTOs;
using Microsoft.EntityFrameworkCore;


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

    [HttpGet("tasks/{listId}")]
    public async Task<IActionResult> GetTasksByListId(int listId)
    {
        var tasks = await this.context.ToDos.Where(t => t.ToDoListId == listId).ToListAsync();
        return this.Ok(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> GetTask()
    {
        var toDo = await this.context.ToDos.ToListAsync();
        return this.Ok(toDo);
    }

    [HttpGet("assigned/{userName}")]
    public async Task<IActionResult> GetTasksAssignedTo(string userName)
    {
        var tasks = await this.context.ToDos
            .Where(t => t.AssignedTo == userName)
            .ToListAsync();

        return this.Ok(tasks);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetFilteredTasks(string category)
    {
        var tasks = await this.context.ToDos
            .Include(t => t.Category)
            .Where(t => t.Category.Name == category)
            .ToListAsync();

        return this.Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskDetails(int id)
    {
        var task = await this.context.ToDos
            .Include(t => t.Category)
            .Include(t => t.Status)
            .FirstOrDefaultAsync(t => t.ToDoId == id);

        if (task == null)
        {
            return this.NotFound();
        }

        return this.Ok(task);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ToDoDTO toDo)
    {
        try
        {
            var existingStatus = await this.context.Statuses
                .FirstOrDefaultAsync(s => s.StatusId == toDo.StatusId);
            if (existingStatus == null)
            {
                return this.BadRequest("Bad Status.");
            }

            var existingCategory = await this.context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == toDo.CategoryId);
            if (existingCategory == null)
            {
                return this.BadRequest("Bad category.");
            }

            if (this.ModelState.IsValid)
            {
                var toDoEntity = new ToDo
                {
                    Description = toDo.Description,
                    DueDate = toDo.DueDate,
                    StatusId = toDo.StatusId,
                    CategoryId = toDo.CategoryId,
                    ToDoListId = toDo.ToDoListId,
                    CreatedBy = toDo.CreatedBy,
                    AssignedTo = toDo.AssignedTo,
                };

                _ = this.context.ToDos.Add(toDoEntity);
                _ = await this.context.SaveChangesAsync();

                return this.Ok(toDoEntity);
            }

            return this.BadRequest(this.ModelState);
        }
        catch (DbUpdateException)
        {
            return this.StatusCode(500, "not add new todo.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var toDo = await this.context.ToDos.FindAsync(id);

        if (toDo == null)
        {
            return this.NotFound("Завдання з таким ID не знайдено.");
        }

        _ = this.context.ToDos.Remove(toDo);
        _ = await this.context.SaveChangesAsync();

        return this.NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ToDoDTO updatedToDo)
    {
        var existingToDo = await this.context.ToDos
            .Include(t => t.Category)
            .Include(t => t.Status)
            .FirstOrDefaultAsync(t => t.ToDoId == id);

        if (existingToDo == null)
        {
            return this.NotFound("Завдання з таким ID не знайдено.");
        }

        var existingCategory = await this.context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == updatedToDo.CategoryId);
        if (existingCategory == null)
        {
            return this.BadRequest("Невірна категорія.");
        }

        var existingStatus = await this.context.Statuses
            .FirstOrDefaultAsync(s => s.StatusId == updatedToDo.StatusId);
        if (existingStatus == null)
        {
            return this.BadRequest("Невірний статус.");
        }

        existingToDo.Description = updatedToDo.Description;
        existingToDo.DueDate = updatedToDo.DueDate;
        existingToDo.CategoryId = updatedToDo.CategoryId;
        existingToDo.StatusId = updatedToDo.StatusId;
        existingToDo.Category = existingCategory;
        existingToDo.Status = existingStatus;

        _ = await this.context.SaveChangesAsync();

        return this.Ok(existingToDo);
    }
}
