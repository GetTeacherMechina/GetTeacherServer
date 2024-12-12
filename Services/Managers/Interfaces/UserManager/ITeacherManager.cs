using GetTeacherServer.Services.Database.Models;

namespace GetTeacherServer.Services.Managers.Interfaces.UserManager;

public interface ITeacherManager
{
    public int[] GetAllTeacherIDs();

    public int[] GetAllTeacherIDsBySubject(int subjectID);

    public int[] GetAllTeacherIDsBySubjectAndStudingLevel(int subjectID, int studyingLevel);

    public void AddTeacher(int userID, int[] subjectIDs, int[] teachingLevel);

    public void RemoveTeacher(int teacherID);

    public int GetTeacherIDByUserID(int userID);

    public double GetTeacherRank(int teacherID);

    public int GetNumOfTeacherRankers(int teacherID);

    public void UpdateTeacherRank(int teacherID, double newRank);

    public void UpdateNumOfTeacherRankers(int teacherID);

    public DbSubject[] GetAllSubjectIDs();
}