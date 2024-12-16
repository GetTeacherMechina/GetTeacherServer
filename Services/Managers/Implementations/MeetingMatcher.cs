using GetTeacherServer.Services.Database.Models;
using GetTeacherServer.Services.Managers.Interfaces;
using GetTeacherServer.Services.Managers.Interfaces.UserManager;

namespace GetTeacherServer.Services.Managers.Implementation;

public class MeetingMatcher : IMeetingMatcher
{
    private readonly IUserStateChecker userStateChecker;
    private readonly IStudentManager studentManager;
    private readonly ITeacherManager teacherManager;
    private readonly ITeacherRankManager rankSystem;

    public MeetingMatcher(IUserStateChecker userStateChecker, IStudentManager studentManager, ITeacherManager teacherManager, ITeacherRankManager rankSystem)
    {
        this.userStateChecker = userStateChecker;
        this.studentManager = studentManager;
        this.teacherManager = teacherManager;
        this.rankSystem = rankSystem;
    }

    public async Task<DbTeacher?> MatchStudentTeacher(DbStudent student, DbSubject subject)
    {
        ICollection<DbTeacher> favoriteTeachers = student.FavoriteTeachers;
        ICollection<DbTeacher> bestTeachersBySubject = await teacherManager.GetTeachersBySubjectAndGrade(subject, student.Grade);

        foreach (DbTeacher currentTeacher in favoriteTeachers)
            if (userStateChecker.IsUserOnline(currentTeacher.DbUser))
                return currentTeacher;

        foreach (DbTeacher currentTeacher in bestTeachersBySubject)
            if (userStateChecker.IsUserOnline(currentTeacher.DbUser))
                return currentTeacher;

        // TODO
        // NotifyOnlineTeachers(bestTeachersBySubject);
        return null;
    }
}
