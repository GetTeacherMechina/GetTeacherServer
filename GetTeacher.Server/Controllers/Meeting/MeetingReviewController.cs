using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("/api/v1/meeting/info")]
public class MeetingReviewController(IStudentCreditCharger studentCreditCharger , IMeetingManager meetingManager) : Controller
{
	private readonly IStudentCreditCharger studentCreditCharger = studentCreditCharger;
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

		// TODO: Move this to a more proper place
		if (!await studentCreditCharger.ChargeStudent(meeting.Student, meeting))
			return BadRequest();

		return Ok(new { Message = "Successfully updated meeting summary" });
	}
}