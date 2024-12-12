using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class AuthManager : IAuthManager
{
    public readonly int NO_ID = -1;
    private readonly IDbManager dbManager;
    private readonly IUserStateChecker userStateChecker;

    public AuthManager(IDbManager dbManager, IUserStateChecker userStateChecker)
    {
        this.dbManager = dbManager;
        this.userStateChecker = userStateChecker;
    }

    private int Register(string name, string email, string password)
    {
        dbManager.AddUser(name, email, password);
        int userID = dbManager.GetUserID(name, email, password);
        userStateChecker.AddUser(userID, DateTime.Now.Ticks);
        return dbManager.GetUserID(name, email, password);
    }

    public bool RegisterStudent(string name, string email, string password, int studyingLevel)
    {
        int userID = Register(name, email, password);
        if (userID == NO_ID)
            return false;

        dbManager.AddStudent(userID, studyingLevel);
        return true;
    }

    public bool RegisterTeacher(string name, string email, string password, int[] subjectIDs, int[] teachingLevel)
    {
        int userID = Register(name, email, password);
        if (userID == NO_ID)
            return false;

        dbManager.AddTeacher(userID, subjectIDs, teachingLevel);
        return true;
    }

    public int Login(string name, string email, string password)
    {
        return dbManager.GetUserID(name, email, password);
    }
}
