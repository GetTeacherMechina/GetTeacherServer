using GetTeacher.Server.DataStructures.Concurrent;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class UserStateTracker : IUserStateTracker
{
	private static readonly ConcurrentList<int> onlineUsers = [];

	public bool IsUserOnline(DbUser user)
	{
		return onlineUsers.Contains(user.Id);
	}

	public ICollection<DbUser> GetOnlineUsers()
	{
		return [.. onlineUsers.ConvertAll(i => new DbUser { Id = i })];
	}

	public void SetOnline(DbUser user)
	{
		onlineUsers.Add(user.Id);
	}

	public void SetOffline(DbUser user)
	{
		onlineUsers.Remove(user.Id);
	}
}