using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Route("List")]
public class ListController : Controller
{
    private readonly Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public ListController()
    {
        this._client = new HttpClient();
        this._client.BaseAddress = baseAdress;
    }

    [Route("Tasks")]
    [HttpGet("Tasks/{id}")]
    public IActionResult Tasks(int id)
    {
        List<ToDoViewModel> list = [];
        HttpResponseMessage response = this._client.GetAsync(this._client.BaseAddress + $"/ToDo/tasks/{id}").Result;

        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }

        HttpResponseMessage listResponse = this._client.GetAsync(this._client.BaseAddress + $"/ToDoList/{id}").Result;

        if (listResponse.IsSuccessStatusCode)
        {
            string listData = listResponse.Content.ReadAsStringAsync().Result;
            var listInfo = JsonConvert.DeserializeObject<ListToDoViewModel>(listData);
            this.ViewBag.ListName = listInfo?.Name;
        }

        this.ViewBag.ListId = id;

        return this.View(list);
    }

    [Route("Edit")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        HttpResponseMessage response = await this._client.GetAsync(this._client.BaseAddress + $"/TodoList/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return this.NotFound();
        }

        string data = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<ListToDoViewModel>(data);

        return this.View(list);
    }

    [Route("Edit")]
    [HttpPost]
    public async Task<IActionResult> Edit(ListToDoViewModel model)
    {
        var strippedModel = new
        {
            Id = model.Id,
            Name = model.Name
        };

        var json = JsonConvert.SerializeObject(strippedModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this._client.PutAsync(this._client.BaseAddress + $"/TodoList/{model.Id}", content);

        if (response.IsSuccessStatusCode)
        {
            return this.RedirectToAction("Index", "Home");
        }

        this.ModelState.AddModelError("", "Failed to update the list.");
        return this.View(model);
    }

    [Route("Delete")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var deleteResponse = await this._client.DeleteAsync(this._client.BaseAddress + $"/TodoList/{id}");

        if (deleteResponse.IsSuccessStatusCode)
        {
            return this.RedirectToAction("Index", "Home");
        }

        this.ModelState.AddModelError("", "Failed to delete the list.");
        return this.RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(ListToDoViewModel model)
    {
        var strippedModel = new
        {
            Id = model.Id,
            Name = model.Name
        };

        var json = JsonConvert.SerializeObject(strippedModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this._client.PostAsync("/api/TodoList", content);

        if (response.IsSuccessStatusCode)
        {
            return this.RedirectToAction("Index", "Home");
        }

        this.ModelState.AddModelError("", "Failed to create the list.");
        return this.View("Index", new List<ListToDoViewModel>());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? searchQuery)
    {
        var allLists = await this._client.GetFromJsonAsync<IEnumerable<ListToDoViewModel>>(this._client.BaseAddress + "/TodoList");

        if (!string.IsNullOrEmpty(searchQuery))
        {
            allLists = allLists.Where(list => list.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }

        this.ViewBag.SearchQuery = searchQuery;
        return this.View(allLists);
    }
}
