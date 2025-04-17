using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Route("Home")]
public class HomeController : Controller
{
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public HomeController(SignInManager<IdentityUser> signInManager)
    {
        this.signInManager = signInManager;
        this._client = new HttpClient();
        this._client.BaseAddress = baseAdress;
    }

    [Route("Index")]
    public IActionResult Index()
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

        List<ListToDoViewModel> list = [];
        HttpResponseMessage response = this._client.GetAsync(this._client.BaseAddress + "/TodoList").Result;

        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ListToDoViewModel>>(data);
        }
        return this.View(list);
    }
}
