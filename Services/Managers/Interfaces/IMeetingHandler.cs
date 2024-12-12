namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IMeetingHandler
{
    public int StartMeeting(int teacherID, int studentID);

    public void EndMeeting(int meetingID);

    public int GetTeacherIdByMeeting(int meetingID);
}