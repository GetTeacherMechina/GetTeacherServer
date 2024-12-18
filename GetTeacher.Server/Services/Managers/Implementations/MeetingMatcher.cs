using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class MeetingMatcher : IMeetingMatcher
{
    private readonly ITeacherRankManager teacherRankManager;
    private readonly IUserStateChecker userStateChecker;

    public MeetingMatcher(ITeacherRankManager teacherRankManager, IUserStateChecker userStateChecker)
    {
        this.teacherRankManager = teacherRankManager;
        this.userStateChecker = userStateChecker;
    }

    public async Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject)
    {
        ICollection<DbTeacher> rankedTeachers = await teacherRankManager.GetRankedTeachersBySubjectAndGradeAndFavorite(student, subject);

        rankedTeachers = rankedTeachers.Where(t => userStateChecker.IsUserOnline(t.DbUser)).ToList();

        // TODO:
        // NotifyOnlineTeachers(bestTeachersBySubject);
        return rankedTeachers.FirstOrDefault();
    }
}
