using System.Collections.Concurrent;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;

namespace GetTeacher.Server.Services.Managers.Implementations.UserStateTracker;

public class UserStateTracker(IUserStateStatus userStateStatus) : IUserStateTracker
{
	private static readonly ConcurrentDictionary<int, List<Action<int>>> disconnectionCallbacks = [];

	private readonly IUserStateStatus userStateStatus = userStateStatus;

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
	{
		disconnectionCallbacks.Remove(user.Id, out _);
	}

	public void ClearDisconnectActions(DbTeacher teacher)
		=> ClearDisconnectActions(new DbUser { Id = teacher.DbUserId });

	public void ClearDisconnectActions(DbStudent student)
		=> ClearDisconnectActions(new DbUser { Id = student.DbUserId });

	public async Task<ICollection<DbUser>> GetOnlineUsers()
	{
		return await userStateStatus.GetOnlineUsers();
	}

	public async Task<bool> IsUserOnline(DbUser user)
	{
		return await userStateStatus.IsUserOnline(user);
	}

	public async Task SetOffline(DbUser user)
	{
		await userStateStatus.SetOffline(user);
		if (disconnectionCallbacks.TryRemove(user.Id, out List<Action<int>>? onDisconnectedCallbacks))
			onDisconnectedCallbacks?.ForEach(onDisconnect => onDisconnect(user.Id));
	}

	public async Task SetOnline(DbUser user)
	{
		await userStateStatus.SetOnline(user);
	}
}