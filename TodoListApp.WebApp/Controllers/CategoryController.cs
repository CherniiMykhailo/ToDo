using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Route("Category")]
public class CategoryController : Controller
{
    private readonly SignInManager<IdentityUser> signInManager;
    Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public CategoryController(SignInManager<IdentityUser> signInManager)
    {
        this.signInManager = signInManager;
        _client = new HttpClient();
        _client.BaseAddress = baseAdress;
    }

    [Route("Index")]
    public IActionResult Index(string? category)
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
        string endpoint;

        if (string.IsNullOrEmpty(category))
        {
            endpoint = "/ToDo";
        }
        else
        {
            endpoint = $"/ToDo/category/{category}";
        }
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + endpoint).Result;


        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }
        return View(list);
    }
}
