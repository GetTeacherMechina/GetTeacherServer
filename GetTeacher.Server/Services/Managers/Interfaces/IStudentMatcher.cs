using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IStudentMatcher
{
	public Task MatchLoop(DbStudent student, DbSubject subject);
}