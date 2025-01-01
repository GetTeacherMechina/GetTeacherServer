﻿using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IUserStateTracker
{
	public bool IsUserOnline(DbUser user);

	public ICollection<DbUser> GetOnlineUsers();

	public void SetOnline(DbUser user);

	public void SetOffline(DbUser user);

	public void AddDisconnectAction(DbUser user, Action<int> onDisconnect);

	public void RemoveDisconnectAction(DbUser user, Action<int> onDisconnect);
}