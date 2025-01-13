using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.UserState;

public interface IUserStateStatus
{
	public Task<ICollection<DbUser>> GetOnlineUsers();

	public Task<bool> IsUserOnline(DbUser user);

	public Task SetOffline(DbUser user);

	public Task SetOnline(DbUser user);
}