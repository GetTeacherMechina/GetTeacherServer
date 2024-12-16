using System.ComponentModel.DataAnnotations;

namespace GetTeacherServer.Models.Authentication.Register;

public class RegisterRequestModel
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public TeacherRequestModel? Teacher { get; set; } = null;

    public StudentRequestModel? Student { get; set; } = null;

}

public class StudentRequestModel
{
    [Required]
    public string Grade { get; set; } = string.Empty;
}


public class TeacherRequestModel
{
    [Required]
    public string Bio { get; set; } = string.Empty;
}