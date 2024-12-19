namespace GetTeacher.Server.Models.Authentication.Login;

public class LoginResponseModel
{
	public string Result { get; set; } = string.Empty;
	public string JwtToken { get; set; } = string.Empty;
}