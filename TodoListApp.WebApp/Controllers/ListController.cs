using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Route("List")]
public class ListController : Controller
{
    Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public ListController()
    {
        _client = new HttpClient();
        _client.BaseAddress = baseAdress;
    }

    [Route("Edit")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/TodoList/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        string data = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<ListToDoViewModel>(data);

        return View(list);
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

        var response = await _client.PutAsync(_client.BaseAddress + $"/TodoList/{model.Id}", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Failed to update the list.");
        return View(model); // Повертає знову на форму з помилкою
    }

    [Route("Delete")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var deleteResponse = await _client.DeleteAsync(_client.BaseAddress + $"/TodoList/{id}");

        if (deleteResponse.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Failed to delete the list.");
        return RedirectToAction("Index", "Home");
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

        var response = await _client.PostAsync("/api/TodoList", content);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", errorMessage);

            var listResponse = await _client.GetAsync("/api/TodoList");
            if (listResponse.IsSuccessStatusCode)
            {
                var jsonData = await listResponse.Content.ReadAsStringAsync();
                var lists = JsonConvert.DeserializeObject<List<ListToDoViewModel>>(jsonData);
                return View("Index", lists);
            }

            return View("Index", new List<ListToDoViewModel>());
        }

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Failed to create the list.");
        return View("Index", new List<ListToDoViewModel>());
    }
}
