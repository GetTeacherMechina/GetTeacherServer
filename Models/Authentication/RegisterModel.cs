using System.ComponentModel.DataAnnotations;

namespace GetTeacherServer.Models.Authentication;

public class RegisterModel
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public bool RememberMe { get; set; }
}