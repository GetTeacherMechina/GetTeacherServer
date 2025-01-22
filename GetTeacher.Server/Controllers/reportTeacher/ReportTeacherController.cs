using GetTeacher.Server.Models.ReportTeacher;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.reportTeacher;

[ApiController]
[Route("/api/v1/report-teacher-controller")]
public class ReportTeacherController(GetTeacherDbContext getTeacherDbContext, IMeetingManager meetingManager, IStudentManager studentManager) : ControllerBase
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IMeetingManager meetingManager = meetingManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> ReportTeacher([FromBody] ReportTeacherRequestModel request) 
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
			return BadRequest("Student not found");

		DbMeeting? meeting = await meetingManager.GetMeeting(request.MeetingGuid);
		if (meeting is null)
			return BadRequest("Meeting not found");

		meeting.Teacher.Reports.Add(request.ReportContent);
		await getTeacherDbContext.SaveChangesAsync();
		return Ok(new { });
	}
}