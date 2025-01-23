using GetTeacher.Server.Models.MeetingHistory;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.MeetingHistory;

[ApiController]
[Route("/api/v1/[controller]")]
public class MeetingsHistoryController(IMeetingManager meetingManager, ITeacherManager teacherManager, IStudentManager studentManager) : ControllerBase
{
	private readonly IMeetingManager meetingManager = meetingManager;
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpPost]
	public async Task<IActionResult> GetMeetingHistory([FromBody] GetMeetingHistoryRequestModel getMeetingHistoryRequestModel)
	{
		ICollection<DbMeeting> meetings;
		if (getMeetingHistoryRequestModel.IsTeacher)
		{
			DbTeacher? teacher = await teacherManager.GetFromUser(User);
			if (teacher is null)
				return BadRequest("Teacher not found");

			meetings = await meetingManager.GetAllTeacherMeetings(teacher);
		}
		else if (getMeetingHistoryRequestModel.IsStudent)
		{
			DbStudent? student = await studentManager.GetFromUser(User);
			if (student is null)
				return BadRequest("Student not found");

			meetings = await meetingManager.GetAllStudentMeetings(student);
		}
		else
			return BadRequest("No meeting history target ticked");

		meetings = [.. meetings.OrderByDescending(x => x.EndTime)];
		ICollection<MeetingHistoryModel> meetingHistory = meetings.Select(x => new MeetingHistoryModel
		{
			SubjectName = x.Subject.Name,
			PartnerName = getMeetingHistoryRequestModel.IsTeacher ? x.Student.DbUser.UserName! : x.Teacher.DbUser.UserName!,
			StartTime = x.StartTime,
			EndTime = x.EndTime,
		}).ToList();

		return Ok(
			new MeetingsHistoryModel
			{
				History = meetingHistory
			}
		);
	}
}