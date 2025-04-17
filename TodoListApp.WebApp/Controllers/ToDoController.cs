using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models.ViewModels;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Controllers;

[Route("ToDo")]
public class ToDoController : Controller
{

    private readonly SignInManager<IdentityUser> signInManager;
    private readonly Uri baseAdress = new Uri("https://localhost:5001/api");
    private readonly HttpClient _client;

    public ToDoController(SignInManager<IdentityUser> signInManager)
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

        List<ToDoViewModel> list = [];
        HttpResponseMessage response = this._client.GetAsync(this._client.BaseAddress + "/ToDo").Result;

        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<List<ToDoViewModel>>(data);
        }
        return this.View(list);
    }

    [Route("Edit")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        HttpResponseMessage response = await this._client.GetAsync(this._client.BaseAddress + $"/ToDo/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return this.NotFound();
        }

        string data = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<ToDoViewModel>(data);

        return this.View(list);
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

        var response = await this._client.PutAsync(this._client.BaseAddress + $"/ToDo/{model.ToDoId}", content);

        if (response.IsSuccessStatusCode)
        {
            if (model.TodoListId > 0)
            {
                return this.RedirectToAction("Tasks", "List", new { id = model.TodoListId });
            }
            return this.RedirectToAction("Index", "Assign");
        }

        this.ModelState.AddModelError("", "Failed to update the list.");
        return this.View(model);
    }

    [Route("Delete")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id, int? TodoListId, string? returnUrl)
    {
        var deleteResponse = await this._client.DeleteAsync(this._client.BaseAddress + $"/ToDo/{id}");

        if (deleteResponse.IsSuccessStatusCode)
        {
            return this.Redirect(returnUrl);
        }


        this.ModelState.AddModelError("", "Failed to delete the list.");
        if (TodoListId.HasValue)
        {
            return this.RedirectToAction("Tasks", "List", new { id = TodoListId.Value });
        }
        return this.RedirectToAction("Index", "Assign");
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

        var response = await this._client.PostAsync("/api/ToDo", content);

        if (response.IsSuccessStatusCode)
        {
            return this.Redirect(model.ReturnUrl);
        }

        this.ModelState.AddModelError("", "Failed to create the list.");
        return this.View("Index", new List<ToDoViewModel>());
    }
}
