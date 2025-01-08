using System.Collections.Concurrent;
using GetTeacher.Server.DataStructures.Concurrent;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class UserStateTracker : IUserStateTracker
{
	private static readonly ConcurrentList<int> onlineUsers = [];
	private static readonly ConcurrentDictionary<int, List<Action<int>>> disconnectionCallbacks = [];

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
		if (disconnectionCallbacks.TryRemove(user.Id, out List<Action<int>>? onDisconnectedCallbacks))
			onDisconnectedCallbacks?.ForEach(onDisconnect => onDisconnect(user.Id));
	}

	public void AddDisconnectAction(DbUser user, Action<int> onDisconnect)
	{
		if (!disconnectionCallbacks.TryGetValue(user.Id, out List<Action<int>>? onDisconnectedCallbacks))
			onDisconnectedCallbacks = disconnectionCallbacks.AddOrUpdate(user.Id, (i) => [], (i, k) => []);

		onDisconnectedCallbacks.Add(onDisconnect);
	}
	public void AddDisconnectAction(DbTeacher teacher, Action<int> onDisconnect)
		=> AddDisconnectAction(new DbUser { Id = teacher.DbUserId }, onDisconnect);

	public void AddDisconnectAction(DbStudent student, Action<int> onDisconnect)
		=> AddDisconnectAction(new DbUser { Id = student.DbUserId }, onDisconnect);

	public void ClearDisconnectActions(DbUser user)
		=> disconnectionCallbacks.Remove(user.Id, out _);

	public void ClearDisconnectActions(DbTeacher teacher)
		=> ClearDisconnectActions(new DbUser { Id = teacher.DbUserId });

	public void ClearDisconnectActions(DbStudent student)
		=> ClearDisconnectActions(new DbUser { Id = student.DbUserId });
}