using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;

public interface ITeacherReadyManager
{
	public ICollection<DbTeacher> GetReadyTeachersForSubjectAndGrade(DbSubject subject, DbGrade grade);
	public void ReadyToTeachSubject(DbTeacher teacher);
	public void NotReadyToTeach(DbTeacher teacher);
}