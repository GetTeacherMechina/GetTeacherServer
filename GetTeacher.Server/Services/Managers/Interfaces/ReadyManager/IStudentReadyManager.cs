using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;

public interface IStudentReadyManager
{
	public void ReadyToStart(DbStudent student, DbSubject subject);
	public void NotReadyToStart(DbStudent student);
}