using System.ComponentModel.DataAnnotations;

namespace GetTeacher.Server.Models.Authentication.Register;

public class RegisterRequestModel
{
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	public required string FullName { get; set; } = string.Empty;

	public required string Password { get; set; } = string.Empty;

	public TeacherRequestModel? Teacher { get; set; } = null;

	public StudentRequestModel? Student { get; set; } = null;

}

public class StudentRequestModel
{
	public required string Grade { get; set; } = string.Empty;
}


public class TeacherRequestModel
{
	public required string Bio { get; set; } = string.Empty;
}