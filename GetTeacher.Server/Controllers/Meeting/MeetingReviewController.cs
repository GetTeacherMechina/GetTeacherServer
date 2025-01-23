using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("/api/v1/meeting/info")]
public class MeetingReviewController(IMeetingManager meetingManager) : Controller
{
	private readonly IMeetingManager meetingManager = meetingManager;

	[HttpPost]
	[Authorize]
	[Route("rate")]
	public async Task<IActionResult> AddRating([FromBody] UpdateRankRequestModel model)
	{
		DbMeeting? meeting = await meetingManager.GetMeeting(model.Guid);
		if (meeting is null)
			return BadRequest();

		await meetingManager.AddStarsReview(model.Guid, model.Rating);
		return Ok(new { Message = "Successfully updated meeting summary" });
	}
}