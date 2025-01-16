using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface ISubjectManager
{
	public Task<ICollection<DbSubject>> GetAllSubjects();

	public Task AddSubject(DbSubject subject);

	public Task<DbSubject?> GetFromName(string name);
}