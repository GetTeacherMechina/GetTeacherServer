using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Managers.Interfaces.UserManager;
using Microsoft.EntityFrameworkCore;

namespace GetTeacherServer.Services.Managers.Implementations.UserManager;

public class StudentManager : IStudentManager
{
    private readonly GetTeacherDbContext getTeacherDbContext;
    private readonly ITeacherManager teacherManager;

    public StudentManager(GetTeacherDbContext getTeacherDbContext, ITeacherManager teacherManager)
    {
        this.getTeacherDbContext = getTeacherDbContext;
        this.teacherManager = teacherManager;
    }

    private async Task<DbStudent?> GetFromUser(DbUser studentUser)
    {
        return await getTeacherDbContext.Students.Where(u => u.DbUser.Id == studentUser.Id).FirstOrDefaultAsync();
    }

    public async Task<bool> StudentExists(DbUser studentUser)
    {
        return await GetFromUser(studentUser) is not null;
    }

    public async Task AddStudent(DbUser user, DbStudent student)
    {
        if (await GetFromUser(user) is not null)
            return;

        getTeacherDbContext.Students.Add(student);
        await getTeacherDbContext.SaveChangesAsync();
    }

    public async Task RemoveStudent(DbUser user)
    {
        DbStudent? student = await GetFromUser(user);
        if (student is null)
            return;

        getTeacherDbContext.Students.Remove(student);
        await getTeacherDbContext.SaveChangesAsync();
    }

    public async Task AddFavoriteTeacher(DbUser studentUser, DbUser teacherUser)
    {
        DbStudent? student = await GetFromUser(studentUser);
        if (student is null)
            return;

        DbTeacher? teacher = await teacherManager.GetFromUser(teacherUser);
        if (teacher is null)
            return;

        student.PrefferedTeachers.Add(teacher);
        await getTeacherDbContext.SaveChangesAsync();
    }

    public async Task RemoveFavoriteTeacher(DbUser studentUser, DbUser teacherUser)
    {
        DbStudent? student = await GetFromUser(studentUser);
        if (student is null)
            return;

        DbTeacher? teacher = await teacherManager.GetFromUser(teacherUser);
        if (teacher is null)
            return;

        student.PrefferedTeachers.Remove(teacher);
        await getTeacherDbContext.SaveChangesAsync();
    }

    public async Task<ICollection<DbTeacher>> GetFavoriteTeachers(DbUser user)
    {
        DbStudent? student = await GetFromUser(user);
        if (student is null)
            return Array.Empty<DbTeacher>();
        
        return student.PrefferedTeachers;
    }

    public async Task GetGrade(DbUser user)
    {
        DbStudent? student = await GetFromUser(user);
        if (student is null)
            return;
    }
}