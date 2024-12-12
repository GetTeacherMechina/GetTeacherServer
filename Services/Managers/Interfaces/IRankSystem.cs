namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IRankSystem
{
    public double GetTeacherRank(int teacherID);

    public int[] GetRankedTeachersBySubject(int subject, int studyingLevel);

    public void RankTeacher(int teacherID, int stars);
}