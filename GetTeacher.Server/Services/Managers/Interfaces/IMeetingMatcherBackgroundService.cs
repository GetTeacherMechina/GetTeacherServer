using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IMeetingMatcherBackgroundService
{
	public void MatchStudent(DbStudent student, DbSubject subject);
}