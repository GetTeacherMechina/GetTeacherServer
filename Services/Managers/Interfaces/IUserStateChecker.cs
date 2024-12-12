namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IUserStateChecker
{
    public bool IsTeacherOnline(int teacherID);

    public int[] GetAllOnlineTeachersBySubject(int studentID, int subjectID);

    public int[] GetAllOnlineTeachers();

    public void UpdateOnlineUsers(int userID, long time);

    public void AddUser(int userID, long time);
}