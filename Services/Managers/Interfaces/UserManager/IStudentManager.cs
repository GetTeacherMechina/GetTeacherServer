namespace GetTeacherServer.Services.Managers.Interfaces.UserManager;

public interface IStudentManager
{
    public void AddStudent(int userID, int studyingLevel);

    public int GetStudentIDByUserID(int userID);

    public void RemoveStudent(int studentID);

    public int[] GetStudentFavoriteTeachers(int studentID);

    public void AddStudentFavoriteTeacher(int studentID, int teacherID);

    public void RemoveStudentFavoriteTeacher(int studentID, int teacherID);

    public int GetStudentStudyingLevelByID(int studentID);
}