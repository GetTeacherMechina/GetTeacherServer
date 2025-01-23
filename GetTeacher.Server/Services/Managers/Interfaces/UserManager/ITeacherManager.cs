using System.Security.Claims;
using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.UserManager;

public interface ITeacherManager
{
	public Task<DbTeacher?> GetFromUser(ClaimsPrincipal user);

	public Task<DbTeacher?> GetFromUser(DbUser user);

	public Task<ICollection<DbTeacher>> GetAllTeachers();

	public Task<ICollection<DbTeacher>> GetTeachersBySubjectAndGrade(DbSubject subject, DbGrade garde);

	public Task AddSubjectToTeacher(DbTeacherSubject subject, DbTeacher teacher);

	public ICollection<DbTeacherSubject> GetAllTeacherSubjects(DbTeacher teacher);

	public Task RemoveSubjectFromTeacher(DbTeacherSubject subject, DbTeacher teacher);

	public Task SetCreditsTariff(DbTeacher teacher, double creditsPerMinute);

	public Task SetBio(DbTeacher teacher, string bio);
}