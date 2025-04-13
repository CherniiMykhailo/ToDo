using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models.ViewModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using NuGet.Protocol.Core.Types;
using TodoListApp.WebApi.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Net;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

[Route("ToDo")]
public class ToDoController : Controller
{
    private readonly AppIdentityDbContext context;
    private readonly SignInManager<IdentityUser> signInManager;
    Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public ToDoController(AppIdentityDbContext ctx, SignInManager<IdentityUser> signInManager)
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

    [Route("Edit")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/ToDo/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        string data = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<ToDoViewModel>(data);

        return View(list);
    }

    [Route("Edit")]
    [HttpPost]
    public async Task<IActionResult> Edit(ToDoViewModel model)
    {
        var strippedModel = new
        {
            ToDoId = model.ToDoId,
            Description = model.Description,
            DueDate = model.DueDate,
            CategoryId = model.CategoryId,
            StatusId = model.StatusId,
        };

        var json = JsonConvert.SerializeObject(strippedModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync(_client.BaseAddress + $"/ToDo/{model.ToDoId}", content);

        if (response.IsSuccessStatusCode)
        {
            if (model.TodoListId > 0)
            {
                return RedirectToAction("Tasks", "List", new { id = model.TodoListId });
            }
            return RedirectToAction("Index", "Assign");
        }

        ModelState.AddModelError("", "Failed to update the list.");
        return View(model);
    }

    [Route("Delete")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id, int? TodoListId)
    {
        var deleteResponse = await _client.DeleteAsync(_client.BaseAddress + $"/ToDo/{id}");

        if (deleteResponse.IsSuccessStatusCode)
        {
            // Якщо передано TodoListId, перенаправляємо на Tasks, інакше - на загальний список
            if (TodoListId.HasValue)
            {
                return RedirectToAction("Tasks", "List", new { id = TodoListId.Value });
            }
            return RedirectToAction("Index", "Assign");
        }


        ModelState.AddModelError("", "Failed to delete the list.");
        if (TodoListId.HasValue)
        {
            return RedirectToAction("Tasks", "List", new { id = TodoListId.Value });
        }
        return RedirectToAction("Index", "Assign");
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(ToDoViewModel model)
    {
        var currentUser = this.User.Identity?.Name ?? "Guest";


        var strippedModel = new
        {
            ToDoId = model.ToDoId,
            Description = model.Description,
            DueDate = model.DueDate,
            CategoryId = model.CategoryId,
            StatusId = model.StatusId,
            AssignedTo = model.AssignedTo ?? currentUser,
            Overdue = model.DueDate < DateTime.Today,
            ToDoListId = model.TodoListId,
            CreatedBy = currentUser,
        };

        var json = JsonConvert.SerializeObject(strippedModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/ToDo", content);

        if (response.IsSuccessStatusCode)
        {
            if (model.TodoListId > 0)
            {
                return RedirectToAction("Tasks", "List", new { id = model.TodoListId });
            }
            return RedirectToAction("Index", "Assign");
        }

        ModelState.AddModelError("", "Failed to create the list.");
        return View("Index", new List<ToDoViewModel>());
    }
}
