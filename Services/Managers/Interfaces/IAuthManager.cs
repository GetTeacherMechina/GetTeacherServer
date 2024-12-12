namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IAuthManager
{
    /*
     Login an existing user.
     Input: name, email, password.
     Output: userID.
    */
    public int Login(string name, string email, string password);

    public bool RegisterStudent(string name, string email, string password, int studyingLevel);

    public bool RegisterTeacher(string name, string email, string password, int[] subjectIDs, int[] teachingLevel);
}