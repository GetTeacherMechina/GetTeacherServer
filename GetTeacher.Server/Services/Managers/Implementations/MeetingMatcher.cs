using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class MeetingMatcher(ITeacherRankManager teacherRankManager, IUserStateTracker userStateChecker) : IMeetingMatcher
{
	private readonly ITeacherRankManager teacherRankManager = teacherRankManager;
	private readonly IUserStateTracker userStateChecker = userStateChecker;

	public async Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject, ICollection<DbTeacher> teacherExclusion)
	{
		ICollection<DbTeacher> rankedTeachers = await teacherRankManager.GetRankedTeachersBySubjectAndGradeAndFavorite(student, subject);

		// Filter only online teachers
		ICollection<DbTeacher> filteredTeachers = (await Task.WhenAll(rankedTeachers.Select(async t =>
			new
			{
				Teacher = t,
				IsOnline = await userStateChecker.IsUserOnline(t.DbUser),
				IsExcluded = teacherExclusion.Any(tE => tE.DbUserId == t.DbUserId)
			})))
			.Where(result => result.IsOnline && !result.IsExcluded)
			.Select(result => result.Teacher)
			.ToList();

		// TODO:
		// Add student preference slider algorithm
		// NotifyOnlineTeachers(bestTeachersBySubject);
		return filteredTeachers.FirstOrDefault();
	}
}