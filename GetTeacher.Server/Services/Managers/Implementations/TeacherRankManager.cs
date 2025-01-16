using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class TeacherRankManager(GetTeacherDbContext getTeacherDbContext, ITeacherManager teacherManager) : ITeacherRankManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly ITeacherManager teacherManager = teacherManager;

	// TODO: Consider ranker
	public async Task AddRatingReview(DbTeacher teacher, DbStudent ranker, int stars)
	{
		DbTeacher? dbTeacher = await teacherManager.GetFromUser(new DbUser { Id = teacher.Id });
		if (dbTeacher is null)
			return;

		double currentRank = dbTeacher.Rank;
		int numOfRankers = dbTeacher.NumOfRankers;
		double newRank = (currentRank * numOfRankers + stars) / (numOfRankers + 1);

		dbTeacher.Rank = newRank;
		dbTeacher.NumOfRankers++;
		await getTeacherDbContext.SaveChangesAsync();
	}
}