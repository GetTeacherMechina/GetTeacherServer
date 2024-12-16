using GetTeacherServer.Services.Database.Models;

namespace GetTeacherServer.Services.Managers.Interfaces.UserManager;

public interface IStudentManager
{
    public Task<DbStudent?> GetFromUser(DbUser studentUser);

    public Task<bool> StudentExists(DbUser studentUser);

    public Task AddStudent(DbUser studentUser, DbStudent student);

    public Task RemoveStudent(DbStudent student);

    public Task AddFavoriteTeacher(DbStudent student, DbTeacher teacher);

    public Task RemoveFavoriteTeacher(DbStudent student, DbTeacher teacher);
}