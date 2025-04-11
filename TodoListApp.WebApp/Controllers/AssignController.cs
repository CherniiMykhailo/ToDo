using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TodoListApp.WebApp.Models.ViewModels;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

[Route("Assign")]
public class AssignController : Controller
{
    private readonly AppIdentityDbContext context;
    private readonly SignInManager<IdentityUser> signInManager;
    Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public AssignController(AppIdentityDbContext ctx, SignInManager<IdentityUser> signInManager)
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
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/ToDo/assigned/{User.Identity.Name}").Result;

        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }
        return View(list);
    }
}
