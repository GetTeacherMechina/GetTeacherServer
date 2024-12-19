using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IUserStateChecker
{
	public bool IsUserOnline(DbUser user);

	public void UpdateUserLastSeen(DbUser user, DateTime time);

	public Task<ICollection<DbUser>> GetOnlineUsers();

	public Task<ICollection<DbTeacher>> GetOnlineTeachers();

	public Task<ICollection<DbStudent>> GetOnlineStudents();
}