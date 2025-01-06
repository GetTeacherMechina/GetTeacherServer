using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class TeacherRankManager(ITeacherManager teacherManager, ITeacherReadyManager teacherReadyManager) : ITeacherRankManager
{
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly ITeacherReadyManager teacherReadyManager = teacherReadyManager;

	public async Task<ICollection<DbTeacher>> GetRankedTeachersBySubjectAndGradeAndFavorite(DbStudent student, DbSubject subject)
	{
		ICollection<DbTeacher> teachers = await teacherManager.GetTeachersBySubjectAndGrade(subject, student.Grade);
		ICollection<DbTeacher> favoriteTeachers = student.FavoriteTeachers;

		teachers.OrderByDescending(t => t.Rank);
		favoriteTeachers.OrderByDescending(t => t.Rank);

		teachers = teachers.Except(favoriteTeachers).ToList();

		// Get the IDs of ready teachers
		HashSet<int> readyTeacherIds = teacherReadyManager
			.GetReadyTeachers(subject, student.Grade)
			.Select(readyT => readyT.Id)
			.ToHashSet();

		return favoriteTeachers
			.Concat(teachers)
			.Where(t => readyTeacherIds.Contains(t.Id))
			.ToList();
	}

	public async Task UpdateRank(DbUser teacherUser, int stars)
	{
		DbTeacher? teacher = await teacherManager.GetFromUser(teacherUser);
		if (teacher is null)
			return;

		double currentRank = await teacherManager.GetTeacherRank(teacher);
		int numOfRankers = await teacherManager.GetNumOfTeacherRankers(teacher);
		double newRank = (currentRank * numOfRankers + stars) / (numOfRankers + 1);

		await teacherManager.UpdateTeacherRank(teacher, newRank);
		await teacherManager.IncrementNumOfLessons(teacher);
	}
}