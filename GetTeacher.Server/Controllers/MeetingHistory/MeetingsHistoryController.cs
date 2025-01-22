using GetTeacher.Server.Models.MeetingHistory;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.MeetingHistory;


[Controller]
[Route("/api/v1/MeetingHistory")]
public class MeetingsHistoryController(
	IMeetingManager meetingManager, ITeacherManager teacherManager,
	IStudentManager studentManager) : ControllerBase
{
	private readonly IMeetingManager meetingManager = meetingManager;
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly IStudentManager studentManager = studentManager;

	[HttpGet]
	public async Task<IActionResult> GetMeetingHistory()
	{
		DbMeeting[] meetings;
		DbTeacher? teacher = await teacherManager.GetFromUser(User);
		DbStudent? student = await studentManager.GetFromUser(User);
		if (teacher is not null)
		{
			meetings = (await meetingManager.GetAllTeacherMeetings(teacher)).ToArray();
		} else if (student is not null)
		{
			meetings = (await meetingManager.GetAllStudentMeetings(student)).ToArray();
		}
		else
		{
			return BadRequest("user not found");
		}
		MeetingHistoryModel[] meetingsHistorie = new MeetingHistoryModel[meetings.Length];
		for (int i = 0; i < meetings.Length; i++)
		{
			meetingsHistorie[i] = new MeetingHistoryModel {
				SubjectName = meetings[i].Subject.Name,
				PrtnerName = teacher is not null ? meetings[i].Teacher.DbUser.UserName
					: meetings[i].Student.DbUser.UserName,
				StartTime = meetings[i].StartTime.ToString(),
				EndTime = meetings[i].EndTime.ToString(),
			};
		}

		return Ok(
			new MeetingsHistoryModel
			{
				History = meetingsHistorie.ToList()
			}
		);
	}
}
