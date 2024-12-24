using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class UserStateChecker : IUserStateChecker
{
	private static readonly IDictionary<int, bool> LastSeenUsers = new Dictionary<int, bool>();

	private readonly GetTeacherDbContext getTeacherDbContext;

	public UserStateChecker(GetTeacherDbContext getTeacherDbContext)
	{
		this.getTeacherDbContext = getTeacherDbContext;
	}

	private List<int> GetOnlineUserIds()
	{
		List<int> userIds = new List<int>(LastSeenUsers.Count);
		foreach (int userId in LastSeenUsers.Keys)
		{
			if (IsUserOnline(new DbUser { Id = userId }))
				userIds.Add(userId);
		}

		return userIds;
	}

	public bool IsUserOnline(DbUser user)
	{
		if (!LastSeenUsers.TryGetValue(user.Id, out bool online))
			return false;

		return online;
	}

	public async Task<ICollection<DbUser>> GetOnlineUsers()
	{
		List<int> onlineUserIds = GetOnlineUserIds();
		return await getTeacherDbContext.Users
			.Where(u => onlineUserIds.Contains(u.Id))
			.ToListAsync();
	}

	public async Task<ICollection<DbTeacher>> GetOnlineTeachers()
	{
		List<int> onlineUserIds = GetOnlineUserIds();
		return await getTeacherDbContext.Teachers
			.Where(t => onlineUserIds.Contains(t.Id))
			.ToListAsync();
	}

	public async Task<ICollection<DbStudent>> GetOnlineStudents()
	{
		List<int> onlineUserIds = GetOnlineUserIds();
		return await getTeacherDbContext.Students
			.Where(s => onlineUserIds.Contains(s.Id))
			.ToListAsync();
	}

	public void UpdateUserOnline(DbUser user, bool online)
	{
		LastSeenUsers[user.Id] = online;
	}
}