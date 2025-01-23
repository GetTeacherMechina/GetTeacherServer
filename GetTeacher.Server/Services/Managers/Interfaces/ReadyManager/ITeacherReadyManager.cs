using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;

public record SubjectReadyTeachersDescriptor(DbSubject Subject, DbGrade Grade, int ReadyTeachersCount);

public interface ITeacherReadyManager
{
	public Task<ICollection<SubjectReadyTeachersDescriptor>> GetReadyTeachersDescriptors();
	public ICollection<DbTeacher> GetReadyTeachersForSubjectAndGrade(DbSubject subject, DbGrade grade);
	public void ReadyToTeachSubject(DbTeacher teacher);
	public void NotReadyToTeach(DbTeacher teacher);
}