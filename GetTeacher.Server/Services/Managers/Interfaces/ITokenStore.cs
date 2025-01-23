namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface ITokenStore
{
	public string CreateCodeForToken(string token);

	public string? GetToken(string code);

	public void RemoveCode(string code);
}