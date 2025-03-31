using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string? Name { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string? ConfirmPassword { get; set; }

    public string ReturnUrl { get; set; } = "/";
}
