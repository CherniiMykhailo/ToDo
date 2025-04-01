using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ToDoListContext _context;

        public IndexModel(ToDoListContext context)
        {
            _context = context;
        }

        public List<ToDo> Tasks { get; set; } = new();

        public async Task OnGetAsync()
        {
            Tasks = await _context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostMarkCompleteAsync(int id)
        {
            var task = await _context.ToDos.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.StatusId = "completed";  // Оновлюємо статус
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
