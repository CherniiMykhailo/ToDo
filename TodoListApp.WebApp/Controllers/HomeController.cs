using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ToDoContext context;

    public HomeController(ToDoContext ctx)
    {
        this.context = ctx;
    }

    public IActionResult Index(string id)
    {
        var filters = new TaskFilters(id);
        this.ViewBag.TaskFilters = filters;

        this.ViewBag.Categories = this.context.Categories.ToList();
        this.ViewBag.Statuses = this.context.Statuses.ToList();
        this.ViewBag.DueFilters = TaskFilters.DueFilterValues;

        IQueryable<ToDo> query = this.context.ToDos
            .Include(t => t.Category)
            .Include(t => t.Status);

        if (filters.HasCategory)
        {
            query = query.Where(t => t.CategoryId == filters.CategoryId);
        }

        if (filters.HasStatus)
        {
            query = query.Where(t => t.StatusId == filters.StatusId);
        }

        if (filters.HasDue)
        {
            var today = DateTime.Today;
            if (filters.IsPast)
            {
                query = query.Where(t => t.DueDate < today);
            }
            else if (filters.IsFuture)
            {
                query = query.Where(t => t.DueDate > today);
            }
            else if (filters.IsToday)
            {
                query = query.Where(t => t.DueDate == today);
            }
        }

        var tasks = query.OrderBy(t => t.DueDate).ToList();

        return this.View(tasks);
    }

    [HttpGet]
    public IActionResult Add()
    {
        this.ViewBag.Categories = this.context.Categories.ToList();
        this.ViewBag.Statuses = this.context.Statuses.ToList();
        var task = new ToDo { StatusId = "open" };
        return this.View(task);
    }

    [HttpPost]
    public IActionResult Add(ToDo task)
    {
        if (this.ModelState.IsValid)
        {
            _ = this.context.Add(task);
            _ = this.context.SaveChanges();
            return this.RedirectToAction("Index");
        }
        else
        {
            this.ViewBag.Categories = this.context.Categories.ToList();
            this.ViewBag.Statuses = this.context.Statuses.ToList();
            return this.View(task);
        }
    }

    [HttpPost]
    public IActionResult Filter(string[] filter)
    {
        string id = string.Join("-", filter);
        return this.RedirectToAction("Index", new { ID = id });
    }

    [HttpPost]
    public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
    {
        selected = this.context.ToDos.Find(selected.ToDoId)!;

        if (selected != null)
        {
            selected.StatusId = "closed";
            _ = this.context.SaveChanges();
        }
        return this.RedirectToAction("Index", new { ID = id });
    }

    public IActionResult DeleteComplete(string id)
    {
        var toDelete = this.context.ToDos.Where(t => t.StatusId == "closed").ToList();

        foreach (var task in toDelete)
        {
            _ = this.context.ToDos.Remove(task);
        }
        _ = this.context.SaveChanges();

        return this.RedirectToAction("Index", new { ID = id });
    }
}
