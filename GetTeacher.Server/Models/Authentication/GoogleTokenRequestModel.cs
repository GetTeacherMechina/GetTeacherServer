namespace GetTeacher.Server.Models.Authentication;

public class GoogleTokenRequestModel
{
	public required string IdToken { get; set; } = string.Empty;
}