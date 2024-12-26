using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class MeetingMatcher(ITeacherRankManager teacherRankManager, IUserStateChecker userStateChecker) : IMeetingMatcher
{
	private readonly ITeacherRankManager teacherRankManager = teacherRankManager;
	private readonly IUserStateChecker userStateChecker = userStateChecker;

	public async Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject)
	{
		ICollection<DbTeacher> rankedTeachers = await teacherRankManager.GetRankedTeachersBySubjectAndGradeAndFavorite(student, subject);

		// Filter only online teachers
		rankedTeachers = rankedTeachers.Where(t => userStateChecker.IsUserOnline(t.DbUser)).ToList();

		// TODO:
		// NotifyOnlineTeachers(bestTeachersBySubject);
		return rankedTeachers.FirstOrDefault();
	}
}