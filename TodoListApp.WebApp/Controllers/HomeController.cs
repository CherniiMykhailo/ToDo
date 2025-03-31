using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

[Route("Home")]
public class HomeController : Controller
{
    private readonly ToDoContext context;
    private readonly SignInManager<IdentityUser> signInManager;

    public HomeController(ToDoContext ctx, SignInManager<IdentityUser> signInManager)
    {
        this.context = ctx;
        this.signInManager = signInManager;
    }

    [Route("Index")]
    public IActionResult Index()
    {
        if (signInManager.IsSignedIn(User))
        {
            ViewData["IsAuthenticated"] = true;
            ViewData["Username"] = User.Identity.Name;
        }
        else
        {
            ViewData["IsAuthenticated"] = false;
        }

        return View(context.ToDos);
    }

    //public async Task<IActionResult> 3Index()
    //{
    //    var client = clientFactory.CreateClient("ToDoAPI");
    //    var response = await client.GetAsync("ToDo");
    //    if (response.IsSuccessStatusCode)
    //    {
    //        var toDos = await response.Content.ReadFromJsonAsync<IEnumerable<ToDo>>();
    //        return View(toDos);
    //    }
    //    return View(new List<ToDo>());
    //}

    public IActionResult Index1(string id)
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

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            if (context.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(user);
            }

            // Додаємо користувача до бази даних
            var newUser = new User
            {
                Username = user.Username,
                Password = Models.User.HashPassword(user.Password)
            };

            context.Users.Add(newUser);
            context.SaveChanges();

            // Перенаправлення на сторінку входу
            return RedirectToAction("Login");
        }

        return View(user);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.ErrorMessage = "Username and password are required.";
            return View();
        }

        var user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == Models.User.HashPassword(password));
        if (user != null)
        {
            // Успішний логін
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.ErrorMessage = "Invalid username or password.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Home");
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
