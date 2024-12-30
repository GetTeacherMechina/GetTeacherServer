namespace GetTeacher.Server.Models.Authentication.Login;

public class LoginRequestModel()
{
	public required string Email { get; set; } = string.Empty;

	public required string Password { get; set; } = string.Empty;
}