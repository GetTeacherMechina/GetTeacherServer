using GetTeacher.Server.Services.Database.Models;
using System.Security.Claims;

namespace GetTeacher.Server.Services.Managers.Interfaces.UserManager;

public interface IUserManager
{
	public Task<DbUser?> GetFromUser(ClaimsPrincipal user);

	public Task<DbUser?> GetFromUser(int userId);
}