using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Meeting;

[Controller]
[Route("/api/v1/meeting/info")]
public class MeetingReviewController(IMeetingManager meetingManager) : Controller
{
	private readonly IMeetingManager meetingManager = meetingManager;

	[HttpPost]
	[Authorize]
	[Route("rate")]
	public async Task<IActionResult> AddRating([FromBody] UpdateRankRequestModel model)
	{
		await meetingManager.AddRatingReview(model.Guid, model.StarsCount);
		return Ok(new { Message = "Successfully updated meeting summary" });
	}
}