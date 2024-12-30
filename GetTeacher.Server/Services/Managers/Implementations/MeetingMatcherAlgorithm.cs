using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class MeetingMatcherAlgorithm(ILogger<IMeetingMatcherAlgorithm> logger, ITeacherReadyManager teacherReadyManager, ITeacherManager teacherManager, IUserStateTracker userStateChecker) : IMeetingMatcherAlgorithm
{
	private readonly ILogger<IMeetingMatcherAlgorithm> logger = logger;
	private readonly ITeacherReadyManager teacherReadyManager = teacherReadyManager;
	private readonly ITeacherManager teacherManager = teacherManager;
	private readonly IUserStateTracker userStateChecker = userStateChecker;

	public async Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject, ICollection<DbTeacher> teacherExclusion)
	{
		HashSet<int> readyTeacherIds = teacherReadyManager
			.GetReadyTeachers(subject, student.Grade)
			.Select(readyT => readyT.Id)
			.ToHashSet();

		// The only source for teachers in this algorithm
		ICollection<DbTeacher> teachers = await teacherManager.GetTeachersBySubjectAndGrade(subject, student.Grade);
		ICollection<DbTeacher> favoriteTeachers = student.FavoriteTeachers;

		teachers = [.. teachers.Except(favoriteTeachers).OrderByDescending(t => t.Rank)];
		favoriteTeachers = [.. favoriteTeachers.OrderByDescending(t => t.Rank)];

		ICollection<DbTeacher> allTeachers = [.. favoriteTeachers, .. teachers];
		ICollection<DbTeacher> onlineTeachers = (await Task.WhenAll(allTeachers.Select(async t =>
			new
			{
				Teacher = t,
				IsReadyToTeach = readyTeacherIds.Contains(t.Id),
				IsOnline = await userStateChecker.IsUserOnline(t.DbUser),
				IsExcluded = teacherExclusion.Any(tE => tE.DbUserId == t.DbUserId),
			})))
			.Where(result => result.IsReadyToTeach && result.IsOnline && !result.IsExcluded)
			.Select(result => result.Teacher)
			.ToList();

		logger.LogDebug("Number of online teachers: {onlineTeachersCount}", onlineTeachers.Count);
		logger.LogDebug("Number of ready teachers: {readyTeachersCount}", readyTeacherIds.Count);
		logger.LogDebug("Number of offline teachers: {offlineTeachersCount}", allTeachers.Count);

		// TODO:
		// Add student preference slider algorithm
		// Add "community contribution" points based on chat activityRemoveSubject
		// NotifyOnlineTeachers(bestTeachersBySubject);
		DbTeacher? matchedTeacher = onlineTeachers.FirstOrDefault();
		if (matchedTeacher is null)
			logger.LogDebug("Matched [Student] {studentName} with no teacher", student.DbUser.UserName);
		else if (matchedTeacher is not null)
			logger.LogDebug("Matched [Student] {studentName} with [Teacher] {teacherName}", student.DbUser.UserName, matchedTeacher.DbUser.UserName);

		return matchedTeacher;
	}
}