using GetTeacher.Server.DataStructures.Concurrent;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;

namespace GetTeacher.Server.Services.Managers.Implementations.UserStateTracker;

public class LocalUserStateStatus : IUserStateStatus
{
	private static readonly ConcurrentList<int> onlineUsers = [];

	public Task<bool> IsUserOnline(DbUser user)
	{
		return Task.FromResult(onlineUsers.Contains(user.Id));
	}

	public Task<ICollection<DbUser>> GetOnlineUsers()
	{
		ICollection<DbUser> onlineDbUsers = [.. onlineUsers.ConvertAll(i => new DbUser { Id = i })];
		return Task.FromResult(onlineDbUsers);
	}

	public Task SetOnline(DbUser user)
	{
		onlineUsers.Add(user.Id);
		return Task.CompletedTask;
	}

	public Task SetOffline(DbUser user)
	{
		onlineUsers.Remove(user.Id);
		return Task.CompletedTask;
	}
}