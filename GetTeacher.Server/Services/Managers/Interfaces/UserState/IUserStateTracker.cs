using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.UserState;

public interface IUserStateTracker
{
	public Task<bool> IsUserOnline(DbUser user);

	public Task<ICollection<DbUser>> GetOnlineUsers();

	public Task SetOnline(DbUser user);

	public Task SetOffline(DbUser user);

	public void AddDisconnectAction(DbUser user, Action<int> onDisconnect);

	public void AddDisconnectAction(DbTeacher teacher, Action<int> onDisconnect);

	public void AddDisconnectAction(DbStudent student, Action<int> onDisconnect);

	public void ClearDisconnectActions(DbUser user);

	public void ClearDisconnectActions(DbTeacher teacher);

	public void ClearDisconnectActions(DbStudent student);
}