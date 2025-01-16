using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IMeetingManager
{
	Task<Guid> AddMeeting(DbTeacher teacher, DbStudent student, DbSubject subject, DbGrade grade);

	Task RemoveMeeting(Guid meetingGuid);

	Task<DbMeeting?> GetMeeting(Guid guid);

	Task<DbMeetingSummary?> GetMeetingSummary(Guid guid);

	Task AddRatingReview(Guid guid, int stars);

	Task<ICollection<DbMeeting>> GetAllStudentMeetings(DbStudent student);

	Task<ICollection<DbMeeting>> GetAllTeacherMeetings(DbTeacher teacher);
}
