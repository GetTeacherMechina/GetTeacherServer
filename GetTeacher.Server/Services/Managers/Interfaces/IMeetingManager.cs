using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IMeetingManager
{
	Task<Guid> AddMeeting(DbTeacher teacher, DbStudent student, DbSubject subject, DbGrade grade);

	Task RemoveMeeting(Guid meetingGuid);

	Task<DbMeeting?> GetMeeting(Guid meetingGuid);

	Task<DbMeetingSummary?> GetMeetingSummary(Guid meetingGuid);

	Task AddStarsReview(Guid meetingGuid, int stars);

	Task<ICollection<DbMeeting>> GetAllStudentMeetings(DbStudent student);

	Task<ICollection<DbMeeting>> GetAllTeacherMeetings(DbTeacher teacher);

	Task<TimeSpan?> GetMeetingLength(Guid meetingGuid);
}
