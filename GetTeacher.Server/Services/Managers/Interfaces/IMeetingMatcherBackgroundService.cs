using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IMeetingMatcherBackgroundService
{
	public void StartMatchStudent(DbStudent student, DbSubject subject);

	public void StopMatchStudent(DbStudent student);
}