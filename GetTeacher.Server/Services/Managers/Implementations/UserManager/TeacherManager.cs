using System.Security.Claims;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations.UserManager;

public class TeacherManager(GetTeacherDbContext getTeacherDbContext, IPrincipalClaimsQuerier principalClaimsQuerier) : ITeacherManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;

	public async Task<DbTeacher?> GetFromUser(ClaimsPrincipal user)
	{
		int? id = principalClaimsQuerier.GetId(user);
		if (id is null)
			return null;

		return await GetFromUser(new DbUser { Id = id.Value });
	}

	public async Task<DbTeacher?> GetFromUser(DbUser user)
	{
		return await getTeacherDbContext.Teachers.Where(t =>
			t.DbUser == new DbUser { Id = user.Id })
				.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Subject)
				.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Grade)
			.FirstOrDefaultAsync();
	}

	public async Task<ICollection<DbTeacher>> GetAllTeachers()
	{
		return await getTeacherDbContext.Teachers.ToListAsync();
	}

	public async Task<ICollection<DbTeacher>> GetTeachersBySubjectAndGrade(DbSubject subject, DbGrade grade)
	{
		return await getTeacherDbContext.Teachers
			.Include(t => t.DbUser)
			.Include(t => t.TeacherSubjects)
				.ThenInclude(tS => tS.Subject)
			.Include(t => t.TeacherSubjects)
				.ThenInclude(tS => tS.Grade)
			.Where(t => t.TeacherSubjects
				.Any(ts => ts.Subject.Name == subject.Name && ts.Grade.Name == grade.Name))
			.ToListAsync();
	}

	public async Task AddSubjectToTeacher(DbTeacherSubject teacherSubject, DbTeacher teacher)
	{
		if (teacher.TeacherSubjects.Any(tS => tS.Subject.Name == teacherSubject.Subject.Name && tS.Grade.Name == teacherSubject.Grade.Name))
			return;

		teacher.TeacherSubjects.Add(teacherSubject);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task RemoveSubjectFromTeacher(DbTeacherSubject subject, DbTeacher teacher)
	{
		teacher.TeacherSubjects.Remove(subject);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public ICollection<DbTeacherSubject> GetAllTeacherSubjects(DbTeacher teacher)
	{
		return teacher.TeacherSubjects;
	}
}