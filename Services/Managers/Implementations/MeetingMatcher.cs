using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class MeetingMatcher : IMeetingMatcher
{
    public readonly int NO_TEACHER = -1;

    private IDbManager dbManager;
    private IUserStateChecker userStateChecker;
    private IFavoriteManager favoriteManager;
    private IRankSystem rankSystem;

    public MeetingMatcher(IDbManager dbManager, IUserStateChecker userStateChecker, IFavoriteManager favoriteManager, IRankSystem rankSystem)
    {
        this.dbManager = dbManager;
        this.userStateChecker = userStateChecker;
        this.favoriteManager = favoriteManager;
        this.rankSystem = rankSystem;
    }

    public int GetTeacherID(int studentID, int subjectID)
    {
        int[] favoriteTeachers = favoriteManager.GetFavoriteTeacherIDsBySubject(studentID, subjectID);
        int[] bestTeachersBySubject = rankSystem.GetRankedTeachersBySubject(subjectID, dbManager.GetStudentStudyingLevelByID(studentID));

        for (int i = 0; i < favoriteTeachers.Length; i++)
            if (userStateChecker.IsTeacherOnline(favoriteTeachers[i]))
                return favoriteTeachers[i];

        for (int i = 0; i < bestTeachersBySubject.Length; i++)
            if (userStateChecker.IsTeacherOnline(bestTeachersBySubject[i]))
                return bestTeachersBySubject[i];

        NotifyOnlineTeachers(bestTeachersBySubject);
        return NO_TEACHER;
    }

    /*
     Notifies online teachers that a student is looking for a lesson in a subject they teach.
     Input student id, subject id.
     Output None.
    */
    private void NotifyOnlineTeachers(int[] teachersID)
    {
        /*TODO*/
    }
}
