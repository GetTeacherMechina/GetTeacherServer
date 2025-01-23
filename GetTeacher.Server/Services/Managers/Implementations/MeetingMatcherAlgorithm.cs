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

	private static ICollection<DbTeacher> SortByRankPriceCommunityScore(ICollection<DbTeacher> teachers, DbStudent student)
	{
		double maxRank = teachers.Count == 0 ? 0 : Math.Max(teachers.Max(t => t.Rank), 1);
		double maxPrice = teachers.Count == 0 ? 0 : Math.Max(teachers.Max(t => t.TariffPerMinute), 1);
		double maxCommunityScore = teachers.Count == 0 ? 0 : Math.Max(teachers.Max(t => t.DbUser.ChatMessagesSent + t.DbUser.ChatMessagesReceived), 1);

		double communityWeightFraction = 0.1;
		double communityWeight = (student.PriceVsQuality / 100.0) * communityWeightFraction;
		double rankWeight = (student.PriceVsQuality / 100.0) * (1 - communityWeightFraction);
		double priceWeight = 1.0 - rankWeight - communityWeight;

		return teachers.Select(t => new
		{
			Teacher = t,
			Score = rankWeight * (t.Rank / maxRank)
				  - priceWeight * (t.TariffPerMinute / maxPrice)
				  + communityWeight * ((t.DbUser.ChatMessagesSent + t.DbUser.ChatMessagesReceived) / maxCommunityScore)
		})
		.OrderByDescending(result => result.Score)
		.Select(result => result.Teacher)
		.ToList();
	}

	public async Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject, ICollection<DbTeacher> teacherExclusion)
	{
		HashSet<int> readyTeacherIds = teacherReadyManager
			.GetReadyTeachersForSubjectAndGrade(subject, student.Grade)
			.Select(readyT => readyT.Id)
			.ToHashSet();

		ICollection<DbTeacher> teachers = await teacherManager.GetTeachersBySubjectAndGrade(subject, student.Grade);
		ICollection<DbTeacher> favoriteTeachers = student.FavoriteTeachers;

		teachers = [.. teachers.Where(t => !favoriteTeachers.Where(f => f.Id == t.Id).Any()).OrderByDescending(t => t.Rank)];
		favoriteTeachers = [.. favoriteTeachers.OrderByDescending(t => t.Rank)];

		teachers = SortByRankPriceCommunityScore(teachers, student);
		favoriteTeachers = SortByRankPriceCommunityScore(favoriteTeachers, student);

		ICollection<DbTeacher> allTeachers = [.. favoriteTeachers, .. teachers];
		DbTeacher? matchedTeacher = (await Task.WhenAll(allTeachers.Select(async t =>
			new
			{
				Teacher = t,
				IsReadyToTeach = readyTeacherIds.Contains(t.Id),
				IsOnline = await userStateChecker.IsUserOnline(t.DbUser),
				IsExcluded = teacherExclusion.Any(tE => tE.DbUserId == t.DbUserId),
			})))
			.Where(result => result.IsReadyToTeach && result.IsOnline && !result.IsExcluded)
			.Select(result => result.Teacher)
			.FirstOrDefault();


		if (matchedTeacher is null)
			logger.LogDebug("Matched [Student] {studentName} with no teacher", student.DbUser.UserName);
		else
			logger.LogDebug("Matched [Student] {studentName} with [Teacher] {teacherName}", student.DbUser.UserName, matchedTeacher.DbUser.UserName);

		return matchedTeacher;
	}
}