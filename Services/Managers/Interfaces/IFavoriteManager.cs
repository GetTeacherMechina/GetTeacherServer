namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IFavoriteManager
{
    public int[] GetFavoriteTeacherIDs(int studentID);

    public int[] GetFavoriteTeacherIDsBySubject(int studentID, int subjectID);
}