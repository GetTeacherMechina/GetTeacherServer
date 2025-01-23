using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IGradeManager
{
	public Task<ICollection<DbGrade>> GetAllGrades();

	public Task AddGrade(DbGrade grade);

	public Task<DbGrade?> GetFromName(string name);
}