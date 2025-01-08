using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;

namespace GetTeacher.Server.Services.Managers.Implementations.ReadyManager;

public class StudentReadyManager(IMeetingMatcherBackgroundService meetingMatcherBackgroundService) : IStudentReadyManager
{
	private readonly IMeetingMatcherBackgroundService meetingMatcherBackgroundService = meetingMatcherBackgroundService;

	public void ReadyToStart(DbStudent student, DbSubject subject)
	{
		meetingMatcherBackgroundService.StartMatchStudent(student, subject);
	}

	public void NotReadyToStart(DbStudent student)
	{
		meetingMatcherBackgroundService.StopMatchStudent(student);
	}
}