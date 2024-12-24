using System.Security.Claims;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IJwtAuthenticator
{
	public ClaimsPrincipal? ValidateToken(string token);
}