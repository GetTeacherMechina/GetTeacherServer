using GetTeacherServer.Services.Database.Models;

namespace GetTeacherServer.Services.Managers.Interfaces;

public interface ITeacherRankManager
{
    public Task<ICollection<DbTeacher>> GetRankedTeachersBySubjectAndGradeAndFavorite(DbStudent student, DbSubject subject);
}