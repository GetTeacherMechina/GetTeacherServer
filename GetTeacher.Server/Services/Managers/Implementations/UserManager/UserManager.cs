using System.Security.Claims;
using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations.UserManager;

public class UserManager(IPrincipalClaimsQuerier principalClaimsQuerier, GetTeacherDbContext getTeacherDbContext) : IUserManager
{
	private readonly IPrincipalClaimsQuerier principalClaimsQuerier = principalClaimsQuerier;
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task<DbUser?> GetFromUser(ClaimsPrincipal user)
	{
		int? id = principalClaimsQuerier.GetId(user);
		if (id is null)
			return null;

		return await GetFromUser(id.Value);
	}

	public async Task<DbUser?> GetFromUser(int userId)
	{
		return await getTeacherDbContext
			.Users
			.Where(u => u.Id == userId)
			.FirstOrDefaultAsync();
	}
}