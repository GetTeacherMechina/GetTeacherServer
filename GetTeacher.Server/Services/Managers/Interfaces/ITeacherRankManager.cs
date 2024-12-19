using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface ITeacherRankManager
{
	public Task<ICollection<DbTeacher>> GetRankedTeachersBySubjectAndGradeAndFavorite(DbStudent student, DbSubject subject);
}