using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface ITeacherRankManager
{
	public Task AddRatingReview(DbTeacher teacher, DbStudent ranker, int stars);
}