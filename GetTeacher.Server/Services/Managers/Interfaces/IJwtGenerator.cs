using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IJwtGenerator
{
	public string? GenerateUserToken(DbUser user);
}