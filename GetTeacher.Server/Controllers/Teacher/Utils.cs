using System.Security.Claims;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Identity;

namespace GetTeacher.Server.Controllers.Teacher;

public class Utils
{
	public async static Task<DbTeacher?> GetTeacherFromUser(
		ClaimsPrincipal User, UserManager<DbUser> userManager, ITeacherManager teacherManager)
	{
		var emailClaim = User.FindFirst(ClaimTypes.Email);
		if (emailClaim is null)
		{
			return null;
		}
		string email = emailClaim.Value;
		DbUser? userResult = await userManager.FindByEmailAsync(email);
		if (userResult is null)
		{
			return null;
		}
		return await teacherManager.GetFromUser(userResult);
	}
}
