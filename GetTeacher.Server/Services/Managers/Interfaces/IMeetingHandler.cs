using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IMeetingHandler
{
    public Task<int> StartMeeting(DbTeacher teacher, DbStudent student);

    public void EndMeeting(int meetingID);

    public Task<int> GetTeacherIdByMeeting(int meetingID);
}