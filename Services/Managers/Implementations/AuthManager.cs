using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class AuthManager : IAuthManager
{
    public readonly int NO_ID = -1;
    private IDb DbM;
    private IUserStateChecker userStateChecker;
    public AuthManager(IDb DBM, IUserStateChecker userStateChecker)
    {
        this.DbM = DBM;
        this.userStateChecker = userStateChecker;
    }

    private int Register(string name, string email, string password)
    {
        DbM.AddUser(name, email, password);
        int userID = DbM.GetUserID(name, email, password);
        userStateChecker.AddUser(userID, DateTime.Now.Ticks);
        return DbM.GetUserID(name, email, password);
    }

    public bool RegisterStudent(string name, string email, string password, int studyingLevel)
    {
        int userID = Register(name, email, password);
        if (userID == NO_ID)
        {
            return false;
        }
        DbM.AddStudent(userID, studyingLevel);
        return true;
    }

    public bool RegisterTeacher(string name, string email, string password, int[] subjectIDs, int[] teachingLevel)
    {
        int userID = Register(name, email, password);
        if (userID == NO_ID)
        {
            return false;
        }
        DbM.AddTeacher(userID, subjectIDs, teachingLevel);
        return true;
    }

    public int Login(string name, string email, string password)
    {
        return DbM.GetUserID(name, email, password);
    }
}
