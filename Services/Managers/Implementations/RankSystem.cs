using GetTeacherServer.Services.Managers.Interfaces;

namespace GetTeacherServer.Services.Managers.Implementation;

public class RankSystem : IRankSystem
{
    private IDbManager DbM;
    public RankSystem(IDbManager DBM)
    {
        this.DbM = DBM;
    }

    public double GetTeacherRank(int teacherID)
    {
        return DbM.GetTeacherRank(teacherID);
    }

    public int[] GetRankedTeachersBySubject(int subject, int studyingLevel)
    {
        int[] teachers = DbM.GetAllTeacherIDsBySubjectAndStudingLevel(subject, studyingLevel);
        double[] rankings = new double[teachers.Length];

        for (int i = 0; i < teachers.Length; i++)
        {
            rankings[i] = GetTeacherRank(teachers[i]);
        }
        Array.Sort(rankings, teachers);
        return teachers;
    }

    public void RankTeacher(int teacherID, int stars)
    {
        double currentRank = DbM.GetTeacherRank(teacherID);
        int numOfRankers = DbM.GetNumOfTeacherRankers(teacherID);
        double newRank = ((currentRank * numOfRankers) + stars) / (numOfRankers + 1);
        DbM.UpdateTeacherRank(teacherID, newRank);
        DbM.UpdateNumOfTeacherRankers(teacherID);
    }
}
