using System.Security.Claims;

namespace GetTeacher.Server.Services.Managers.Interfaces.UserManager;

public interface IPrincipalClaimsQuerier
{
	public int? GetId(ClaimsPrincipal claimsPrincipal);

	public string? GetEmail(ClaimsPrincipal claimsPrincipal);
}