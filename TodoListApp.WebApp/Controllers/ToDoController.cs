using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models.ViewModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TodoListApp.WebApp.Controllers;
public class ToDoController : Controller
{
    Uri baseAdress = new Uri("https://localhost:5001");
    private readonly HttpClient _client;

    public ToDoController()
    {
        _client = new HttpClient();
        _client.BaseAddress = baseAdress;
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<ToDoViewModel> list = new List<ToDoViewModel>();
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/todo").Result;

        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }
        return View(list);
    }
}
