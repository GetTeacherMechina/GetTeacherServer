using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Authentication;


[ApiController]
[Route("api/v1/auth/[controller]")]
public class ValidateController(IPrincipalClaimsQuerier principalClaimsQuerier, IUserStateTracker userStateTracker) : ControllerBase
{
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;
	private readonly IUserStateTracker userStateTracker = userStateTracker;

	[Authorize]
	[HttpPost]
	public IActionResult Validate()
	{
		int? userId = principalClaimsQuerier.GetId(User);
		if (userId is null)
			return Unauthorized(new { });

		if (userStateTracker.IsUserOnline(new Services.Database.Models.DbUser { Id = userId.Value }))
			return BadRequest(new { });

		return Ok(new { });
	}
}