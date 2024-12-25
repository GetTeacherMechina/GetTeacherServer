using System.Security.Claims;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Services.Managers.Implementations.UserManager;

public class PrincipalClaimsQuerier : IPrincipalClaimsQuerier
{
	public int? GetId(ClaimsPrincipal claimsPrincipal)
	{
		var idClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
		if (idClaim is null)
			return null;

		if (!int.TryParse(idClaim.Value, out var id))
			return null;

		return id;
	}
}