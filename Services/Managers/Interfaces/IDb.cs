namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IDb
{
    public int[] GetAllTeacherIDs();

    public int[] GetAllTeacherIDsBySubject(int subjectID);

    public int[] GetAllTeacherIDsBySubjectAndStudingLevel(int subjectID, int studyingLevel);

    public int[] GetAllSubjectIDs();

    public void AddTeacher(int userID, int[] subjectIDs, int[] teachingLevel);

    public void AddStudent(int userID, int studyingLevel);

    public void AddUser(string username, string email, string password);

    public int GetUserID(string username, string email, string password);

    public int GetTeacherIDByUserID(int userID);

    public int GetStudentIDByUserID(int userID);

    public void RemoveTeacher(int teacherID);

    public void RemoveStudent(int studentID);

    public int[] GetStudentFavoriteTeachers(int studentID);

    public void AddStudentFavoriteTeacher(int studentID, int teacherID);

    public void RemoveStudentFavoriteTeacher(int studentID, int teacherID);

    public int GetStudentStudyingLevelByID(int studentID);

    public double GetTeacherRank(int teacherID);

    public int GetNumOfTeacherRankers(int teacherID);

    public void UpdateTeacherRank(int teacherID, double newRank);

    public void UpdatetNumOfTeacherRankers(int teacherID);
}