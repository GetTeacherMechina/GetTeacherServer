using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface ITeacherReadyManager
{
	public ICollection<DbTeacher> GetReadyTeachers(DbSubject subject, DbGrade grade);
	public void ReadyToTeachSubject(DbTeacher teacher);
	public void NotReadyToTeach(DbTeacher teacher);
}