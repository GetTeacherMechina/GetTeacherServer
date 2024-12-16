using GetTeacherServer.Services.Database.Models;

namespace GetTeacherServer.Services.Managers.Interfaces.UserManager;

public interface IStudentManager
{
    public Task<bool> StudentExists(DbUser studentUser);

    public Task AddStudent(DbUser studentUser, DbStudent student);

    public Task RemoveStudent(DbUser studentUser);

    public Task AddFavoriteTeacher(DbUser studentUser, DbUser teacherUser);

    public Task RemoveFavoriteTeacher(DbUser studentUser, DbUser teacherUser);

    public Task<ICollection<DbTeacher>> GetFavoriteTeachers(DbUser studentUser);

    public Task GetGrade(DbUser studentUser);
}