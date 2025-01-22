using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IUserCreditManager
{
	public Task AddCreditsToUser(DbUser user, double credits);

	public Task RemoveCreditsFromUser(DbUser user, double credits);
}