using GetTeacherServer.Services.Database.Models;

namespace GetTeacherServer.Services.Managers.Interfaces;

public interface IMeetingHandler
{
    public Task<int> StartMeeting(DbTeacher teacher, DbStudent student);

    public void EndMeeting(int meetingID);

    public Task<int> GetTeacherIdByMeeting(int meetingID);
}