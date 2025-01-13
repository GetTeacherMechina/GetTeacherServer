using System.Security.Claims;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations.UserManager;

public class StudentManager(GetTeacherDbContext getTeacherDbContext, IPrincipalClaimsQuerier principalClaimsQuerier) : IStudentManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;

	public async Task<DbStudent?> GetFromUser(ClaimsPrincipal user)
	{
		int? id = principalClaimsQuerier.GetId(user);
		if (id is null)
			return null;

		return await GetFromUser(new DbUser { Id = id.Value });
	}

	public async Task<DbStudent?> GetFromUser(DbUser studentUser)
	{
		return await getTeacherDbContext.Students.Where(u => u.DbUser.Id == studentUser.Id).FirstOrDefaultAsync();
	}

	public async Task<bool> StudentExists(DbUser studentUser)
	{
		return await GetFromUser(studentUser) is not null;
	}

	public async Task AddStudent(DbUser user, DbStudent student)
	{
		if (await GetFromUser(user) is not null)
			return;

		getTeacherDbContext.Students.Add(student);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task RemoveStudent(DbStudent student)
	{
		getTeacherDbContext.Students.Remove(student);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task AddFavoriteTeacher(DbStudent student, DbTeacher teacher)
	{
		student.FavoriteTeachers.Add(teacher);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task RemoveFavoriteTeacher(DbStudent student, DbTeacher teacher)
	{
		student.FavoriteTeachers.Remove(teacher);
		await getTeacherDbContext.SaveChangesAsync();
	}
}