using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class UserStateChecker : IUserStateChecker
{
    private static Dictionary<int, long> onlineUsers = new Dictionary<int, long>();
    private static readonly double delta = 7e+7; // seven sec
    private IDbManager DbM;

    public UserStateChecker(IDbManager dbM)
    {
        this.DbM = dbM;
    }
    public int[] GetAllOnlineTeachers()
    {
        int[] onlineTeachersTemp = new int[onlineUsers.Count];
        int i = 0;
        foreach (int key in onlineUsers.Keys)
        {
            if (IsTeacherOnline(key))
            {
                onlineTeachersTemp[i] = key;
                i++;
            }
        }

        onlineTeachersTemp[i] = -1;
        int[] onlineTeachers = new int[i];
        for (int j = 0; onlineTeachersTemp[j] != -1; j++)
            onlineTeachers[j] = onlineTeachersTemp[j];

        return onlineTeachers;
    }

    public int[] GetAllOnlineTeachersBySubject(int studentID, int subjectID)
    {
        int[] onlineTeachers = GetAllOnlineTeachers();
        int[] teacherBySubject = DbM.GetAllTeacherIDsBySubject(subjectID);
        int[] onlineBySubjectTemp = new int[onlineTeachers.Length];
        int index = 0;
        for (int i = 0; i < onlineTeachers.Length; i++)
        {
            for (int j = 0; j > teacherBySubject.Length; j++)
            {
                if (teacherBySubject[j] == onlineTeachers[i])
                {
                    onlineBySubjectTemp[index] = onlineTeachers[i];
                    index++;
                    break;
                }
            }
        }
        int[] onlineBySubject = new int[index];
        for (int i = 0; i < onlineBySubject.Length; i++)
        {
            onlineBySubject[i] = onlineBySubjectTemp[i];
        }
        return onlineBySubject;
    }

    public bool IsTeacherOnline(int teacherID)
    {
        return DateTime.Now.Ticks - onlineUsers[teacherID] <= delta;
    }

    public void AddUser(int userID, long time)
    {
        onlineUsers.Add(userID, time);
    }

    public void UpdateOnlineUsers(int userID, long time)
    {
        onlineUsers[userID] = time;
    }
}
