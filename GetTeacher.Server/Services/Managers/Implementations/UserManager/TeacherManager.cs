using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations.UserManager;

public class TeacherManager(GetTeacherDbContext getTeacherDbContext) : ITeacherManager
{
    private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

    public async Task AddTeacher(DbUser user, DbTeacher teacher)
    {

        if (await GetFromUser(user) is not null)
            return;

        getTeacherDbContext.Teachers.Add(teacher);
        await getTeacherDbContext.SaveChangesAsync();
    }
    public async Task RemoveTeacher(DbTeacher teacher)
    {
        if (!(await TeacherExists(teacher.DbUser)))
            return;

        getTeacherDbContext.Teachers.Remove(teacher);
        await getTeacherDbContext.SaveChangesAsync();
    }

    public async Task<ICollection<DbSubject>> GetAllSubjects()
    {
        return await getTeacherDbContext.Subjects.ToListAsync();
    }

    public async Task<ICollection<DbTeacher>> GetAllTeacher()
    {
        return await getTeacherDbContext.Teachers.ToListAsync();
    }

    public async Task<ICollection<DbTeacher>> GetTeachersBySubjectAndGrade(DbSubject subject, DbGrade garde)
    {
        return await getTeacherDbContext.Teachers.Where(
            t => t.TeacherSubjects.Where(sub => sub.Subject.Id == subject.Id &&
            sub.Grade.Id == garde.Id).Count() != 0).ToListAsync();
    }

    public async Task<DbTeacher?> GetFromUser(DbUser user)
    {
        return await getTeacherDbContext.Teachers.Where(t =>
            t.DbUser == user).FirstOrDefaultAsync();
    }

    public async Task<int> GetNumOfTeacherRankers(DbTeacher teacher)
    {
        if (!await TeacherExists(teacher.DbUser))
            return 0;

        return (await getTeacherDbContext.Teachers.Where(t =>
            t.Id == teacher.Id).FirstAsync()).NumOfLessons;
    }
    public async Task<double> GetTeacherRank(DbTeacher teacher)
    {
        if (!await TeacherExists(teacher.DbUser))
            return 0;

        return (await getTeacherDbContext.Teachers.Where(t =>
            t.Id == teacher.Id).FirstAsync()).Rank;
    }

    public async Task<bool> TeacherExists(DbUser user)
    {
        return await getTeacherDbContext.Teachers.Where(t =>
            t.DbUser.Id == user.Id).FirstOrDefaultAsync() is not null;
    }

    public async Task IncrementNumOfLessons(DbTeacher teacher)
    {
        if (!await TeacherExists(teacher.DbUser))
            return;

        (await getTeacherDbContext.Teachers.Where(t => t.Id == teacher.Id).FirstAsync()).NumOfLessons++;
        await getTeacherDbContext.SaveChangesAsync();
    }

    public async Task UpdateTeacherRank(DbTeacher teacher, double newRank)
    {
        if (!await TeacherExists(teacher.DbUser))
            return;

        (await getTeacherDbContext.Teachers.Where(t => t.Id == teacher.Id).FirstAsync()).Rank = newRank;
        await getTeacherDbContext.SaveChangesAsync();
    }
}
