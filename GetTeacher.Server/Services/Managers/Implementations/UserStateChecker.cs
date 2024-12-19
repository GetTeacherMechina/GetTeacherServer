﻿using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class UserStateChecker(GetTeacherDbContext getTeacherDbContext) : IUserStateChecker
{
    private static readonly IDictionary<int, DateTime> lastSeenUsers = new Dictionary<int, DateTime>();
    private static readonly TimeSpan delta = TimeSpan.FromSeconds(15);

    private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

    private List<int> GetOnlineUserIds()
    {
        List<int> userIds = new List<int>(lastSeenUsers.Count);
        foreach (int userId in lastSeenUsers.Keys)
            if (IsUserOnline(new DbUser { Id = userId }))
                userIds.Add(userId);

        return userIds;
    }

    public bool IsUserOnline(DbUser user)
    {
        if (!lastSeenUsers.TryGetValue(user.Id, out DateTime lastSeenUser))
            return false;

        return DateTime.Now - lastSeenUser <= delta;
    }

    public async Task<ICollection<DbUser>> GetOnlineUsers()
    {
        List<int> onlineUserIds = GetOnlineUserIds();
        return await getTeacherDbContext.Users.Where(u => onlineUserIds.Contains(u.Id)).ToListAsync();
    }

    public async Task<ICollection<DbTeacher>> GetOnlineTeachers()
    {
        List<int> onlineUserIds = GetOnlineUserIds();
        return await getTeacherDbContext.Teachers.Where(t => onlineUserIds.Contains(t.Id)).ToListAsync();
    }

    public async Task<ICollection<DbStudent>> GetOnlineStudents()
    {
        List<int> onlineUserIds = GetOnlineUserIds();
        return await getTeacherDbContext.Students.Where(s => onlineUserIds.Contains(s.Id)).ToListAsync();
    }

    public void UpdateUserLastSeen(DbUser user, DateTime time)
    {
        lastSeenUsers[user.Id] = time;
    }
}
