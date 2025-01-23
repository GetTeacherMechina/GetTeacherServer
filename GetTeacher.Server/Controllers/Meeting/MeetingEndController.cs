using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Meeting;

[ApiController]
[Route("/api/v1/meeting")]
public class MeetingEndController(ICreditCharger studentCreditCharger, IMeetingManager meetingManager, IWebSocketSystem webSocketSystem) : ControllerBase
{
	private readonly ICreditCharger creditCharger = studentCreditCharger;
	private readonly IMeetingManager meetingManager = meetingManager;
	private readonly IWebSocketSystem webSocketSystem = webSocketSystem;

	[HttpPost]
	[Authorize]
	[Route("end")]
	public async Task<IActionResult> EndMeeting([FromBody] EndMeetingRequestModel endMeetingRequestModel)
	{
		DbMeeting? meeting = await meetingManager.GetMeeting(endMeetingRequestModel.MeetingGuid);
		if (meeting is null)
			return BadRequest();

		if (meeting.Ended)
			return BadRequest();

		await meetingManager.EndMeeting(meeting.Guid);

		Task studentEndMeetingSendTask = webSocketSystem.SendAsync(meeting.Student.DbUserId, new EndMeetingResponseModel { Status = "EndMeeting" }, "EndMeeting");
		Task teacherEndMeetingSendTask = webSocketSystem.SendAsync(meeting.Teacher.DbUserId, new EndMeetingResponseModel { Status = "EndMeeting" }, "EndMeeting");
		await Task.WhenAll(studentEndMeetingSendTask, teacherEndMeetingSendTask);

		if (!await creditCharger.MeetingTransaction(meeting.Student, meeting.Teacher, meeting))
			return BadRequest();

		return Ok(new { Status = "Ended meeting" });
	}
}