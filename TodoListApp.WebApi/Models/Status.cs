using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TodoListApp.WebApi.Models
{
    public class Status
    {
        public string StatusId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [ValidateNever]
        public ICollection<ToDo> ToDos { get; set; } = [];
    }
}
