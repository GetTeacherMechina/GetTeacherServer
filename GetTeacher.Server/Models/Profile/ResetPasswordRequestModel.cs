namespace GetTeacher.Server.Models.Profile;

public class ResetPasswordRequestModel
{
	public string Code { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	public string ConfirmPassword { get; set; } = string.Empty;
}