using GetTeacher.Server.Models.Status;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Status;
[ApiController]
[Route("api/v1/status/[controller]")]
public class OnlineUsersController(IUserStateStatus userStateStatus) : ControllerBase
{
	private readonly IUserStateStatus userStateStatus = userStateStatus;

	[HttpGet]
	public async Task<IActionResult> GetOnlineUsers()
	{
		return Ok(new OnlineUsersResponseModel { OnlineUserIds = [.. (await userStateStatus.GetOnlineUsers()).ToList().ConvertAll(u => u.Id)] });
	}
}