using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Route("Home")]
public class HomeController : Controller
{
    private readonly AppIdentityDbContext context;
    private readonly SignInManager<IdentityUser> signInManager;
    Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public HomeController(AppIdentityDbContext ctx, SignInManager<IdentityUser> signInManager)
    {
        this.context = ctx;
        this.signInManager = signInManager;
        _client = new HttpClient();
        _client.BaseAddress = baseAdress;
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

        List<ToDoViewModel> list = new List<ToDoViewModel>();
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/ToDo").Result;

        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }
        return View(list);
    }
}
