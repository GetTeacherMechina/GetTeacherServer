
using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface ITwoFactorAuthenticationManager
{
	Task CreateAndSend2FaCode(DbUser user);

	public Task<bool> Confirm2FaCode(DbUser user, string code);
}