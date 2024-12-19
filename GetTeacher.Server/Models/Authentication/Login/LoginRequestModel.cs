using System.ComponentModel.DataAnnotations;

namespace GetTeacher.Server.Models.Authentication.Login;

public class LoginRequestModel()
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}