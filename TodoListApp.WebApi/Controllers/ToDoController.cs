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



















    //[HttpGet("{id}")]
    //public async Task<ActionResult<ToDo>> GetToDoById(int id)
    //{
    //    var toDo = await context.ToDos.Include(t => t.Category)
    //                                  .Include(t => t.Status)
    //                                  .FirstOrDefaultAsync(t => t.ToDoId == id);

    //    if (toDo == null)
    //    {
    //        return NotFound();
    //    }

    //    return Ok(toDo);
    //}

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<ToDo>>> GetAll()
    //{
    //    var todos = await this.context.ToDos
    //        .Include(t => t.Category) // Якщо є зв'язок із Category
    //        .Include(t => t.Status)  // Якщо є зв'язок із Status
    //        .ToListAsync();

    //    return this.Ok(todos);
    //}

    //public IActionResult GetAll(string id)
    //{
    //    var filters = new TaskFilters(id);
    //    this.ViewBag.TaskFilters = filters;

    //    this.ViewBag.Categories = this.context.Categories.ToList();
    //    this.ViewBag.Statuses = this.context.Statuses.ToList();
    //    this.ViewBag.DueFilters = TaskFilters.DueFilterValues;

    //    IQueryable<ToDo> query = this.context.ToDos
    //        .Include(t => t.Category)
    //        .Include(t => t.Status);

    //    if (filters.HasCategory)
    //    {
    //        query = query.Where(t => t.CategoryId == filters.CategoryId);
    //    }

    //    if (filters.HasStatus)
    //    {
    //        query = query.Where(t => t.StatusId == filters.StatusId);
    //    }

    //    if (filters.HasDue)
    //    {
    //        var today = DateTime.Today;
    //        if (filters.IsPast)
    //        {
    //            query = query.Where(t => t.DueDate < today);
    //        }
    //        else if (filters.IsFuture)
    //        {
    //            query = query.Where(t => t.DueDate > today);
    //        }
    //        else if (filters.IsToday)
    //        {
    //            query = query.Where(t => t.DueDate == today);
    //        }
    //    }

    //    var tasks = query.OrderBy(t => t.DueDate).ToList();

    //    return Ok(tasks);
    //}

    //[HttpGet]
    //public IActionResult Add()
    //{
    //    this.ViewBag.Categories = this.context.Categories.ToList();
    //    this.ViewBag.Statuses = this.context.Statuses.ToList();
    //    var task = new ToDo { StatusId = "open" };
    //    return this.View(task);
    //}

    //[HttpPost]
    //public IActionResult Add(ToDo task)
    //{
    //    if (this.ModelState.IsValid)
    //    {
    //        _ = this.context.Add(task);
    //        _ = this.context.SaveChanges();
    //        return this.RedirectToAction("Index");
    //    }
    //    else
    //    {
    //        this.ViewBag.Categories = this.context.Categories.ToList();
    //        this.ViewBag.Statuses = this.context.Statuses.ToList();
    //        return this.View(task);
    //    }
    //}

    //[HttpPost]
    //public IActionResult Filter(string[] filter)
    //{
    //    string id = string.Join("-", filter);
    //    return this.RedirectToAction("Index", new { ID = id });
    //}

    //[HttpPost]
    //public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
    //{
    //    selected = this.context.ToDos.Find(selected.ToDoId)!;

    //    if (selected != null)
    //    {
    //        selected.StatusId = "closed";
    //        _ = this.context.SaveChanges();
    //    }
    //    return this.RedirectToAction("Index", new { ID = id });
    //}

    //public IActionResult DeleteComplete(string id)
    //{
    //    var toDelete = this.context.ToDos.Where(t => t.StatusId == "closed").ToList();

    //    foreach (var task in toDelete)
    //    {
    //        _ = this.context.ToDos.Remove(task);
    //    }
    //    _ = this.context.SaveChanges();

    //    return this.RedirectToAction("Index", new { ID = id });
    //}
}

