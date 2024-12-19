using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class TeacherRankManager(ITeacherManager teacherManager) : ITeacherRankManager
{
	private readonly ITeacherManager teacherManager = teacherManager;

	public async Task<ICollection<DbTeacher>> GetRankedTeachersBySubjectAndGradeAndFavorite(DbStudent student, DbSubject subject)
	{
		ICollection<DbTeacher> teachers = await teacherManager.GetTeachersBySubjectAndGrade(subject, student.Grade);
		ICollection<DbTeacher> favoriteTeachers = student.FavoriteTeachers;

		teachers.OrderByDescending(t => t.Rank);
		favoriteTeachers.OrderByDescending(t => t.Rank);

		teachers = teachers.Except(favoriteTeachers).ToList();

		return favoriteTeachers.Concat(teachers).ToList();
	}

	public async Task UpdateRank(DbUser teacherUser, int stars)
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(teacherUser);
		if (teacher is null)
			return;

		double currentRank = await teacherManager.GetTeacherRank(teacher);
		int numOfRankers = await teacherManager.GetNumOfTeacherRankers(teacher);
		double newRank = ((currentRank * numOfRankers) + stars) / (numOfRankers + 1);

		await teacherManager.UpdateTeacherRank(teacher, newRank);
		await teacherManager.IncrementNumOfLessons(teacher);
	}
}