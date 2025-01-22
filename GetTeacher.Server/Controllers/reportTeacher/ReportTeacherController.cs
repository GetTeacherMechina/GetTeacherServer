using GetTeacher.Server.Models.ReportTeacher;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace GetTeacher.Server.Controllers.reportTeacher;

[ApiController]
[Route("/api/v1/Report-teacher-controller")]
public class ReportTeacherController(
	IMeetingManager meetingManager, IStudentManager studentManager) : ControllerBase
{
	private readonly IMeetingManager meetingManager = meetingManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpPost]
	
	public async Task<IActionResult> ReportLastTeacher([FromBody] ReportTeacherModel request) 
	{
		DbStudent? student = await studentManager.GetFromUser(User);
		if (student is null)
		{
			return BadRequest("student not found");
		}
		ICollection<DbMeeting> meetings = await meetingManager.GetAllStudentMeetings(student);
		DbTeacher teacher = meetings.OrderByDescending(a => a.EndTime).ToArray()[0].Teacher;
		teacher.Reports.Add(request.report);
		return Ok(new { });
	}
}
