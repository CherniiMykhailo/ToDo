using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TodoListApp.WebApi.Models
{
    public class ToDo
    {
        [Key]
        public int ToDoId { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a due time.")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Please enter a category.")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonIgnore]
        public Category Category { get; set; } = null!;

        [Required(ErrorMessage = "Please enter a status.")]
        public string StatusId { get; set; } = string.Empty;

        [JsonIgnore]
        public Status Status { get; set; } = null!;

        public int? ToDoListId { get; set; }

        [JsonIgnore]
        public ToDoList? ToDoList { get; set; }

        public bool Overdue => this.StatusId == "open" && this.DueDate < DateTime.Today;

        public string? CreatedBy { get; set; }

        public string? AssignedTo { get; set; }
    }
}
