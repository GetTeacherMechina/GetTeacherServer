using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class UserCreditManager(GetTeacherDbContext getTeacherDbContext) : IUserCreditManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task AddCreditsToUser(DbUser user, double credits)
	{
		user.Credits += credits;
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task RemoveCreditsFromUser(DbUser user, double credits)
	{
		user.Credits -= credits;
		await getTeacherDbContext.SaveChangesAsync();
	}
}