namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IMeetingMatcher
{
    /*
     Look for a teacher based on the selected subject and the student's studying level.
     If no such teacher is available, call NotifyOnlineTeachers and wait for a connection.
     Input student id, subject id.
     Output selected teacher id.
    */
    public int GetTeacherID(int studentID, int subjectID);

}