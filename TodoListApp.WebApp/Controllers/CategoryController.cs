using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Route("Category")]
public class CategoryController : Controller
{
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public CategoryController(SignInManager<IdentityUser> signInManager)
    {
        this.signInManager = signInManager;
        this._client = new HttpClient();
        this._client.BaseAddress = this.baseAdress;
    }

    [Route("Index")]
    public IActionResult Index(string? category)
    {
        if (this.signInManager.IsSignedIn(this.User))
        {
            this.ViewData["IsAuthenticated"] = true;
            this.ViewData["Username"] = this.User.Identity.Name;
        }
        else
        {
            this.ViewData["IsAuthenticated"] = false;
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
        HttpResponseMessage response = this._client.GetAsync(this._client.BaseAddress + endpoint).Result;


        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }
        return this.View(list);
    }
}
