using System.ComponentModel.DataAnnotations;

namespace GetTeacherServer.Models.Authentication.Register;

public class RegisterRequestModel
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}