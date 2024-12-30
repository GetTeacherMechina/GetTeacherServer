using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class MeetingManager(ILogger<IMeetingManager> logger, GetTeacherDbContext getTeacherDbContext) : IMeetingManager
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;

	public async Task<Guid> AddMeeting(DbTeacher teacher, DbStudent student, DbSubject subject, DbGrade grade)
	{
		Guid guid = Guid.NewGuid();
		DateTime startTime = DateTime.UtcNow;

		logger.LogDebug("Adding a new meeting: [Student:{studentName}] [Teacher:{teacherName}] [Guid:{guid}]", student.DbUser.UserName, teacher.DbUser.UserName, guid);
		DbMeeting meeting = new DbMeeting
		{
			Guid = guid,
			StudentId = student.Id,
			TeacherId = teacher.Id,
			SubjectId = subject.Id,
			GradeId = grade.Id,
			StartTime = startTime
		};

		await getTeacherDbContext.Meetings.AddAsync(meeting);
		await getTeacherDbContext.SaveChangesAsync();
		return guid;
	}

	public async Task RemoveMeeting(Guid meetingGuid)
	{
		DbMeeting? toRemove = getTeacherDbContext.Meetings.Where(m => m.Guid == meetingGuid).FirstOrDefault();
		if (toRemove is null)
			return;

		getTeacherDbContext.Meetings.Remove(toRemove);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task AddRatingReview(Guid guid, int starsCount)
	{
		DbMeeting? meeting = await GetMeeting(guid);
		if (meeting is null)
		{
			logger.LogWarning("Meeting was not found.");
			return;
		}

		if (meeting.MeetingSummary is not null)
		{
			logger.LogWarning("Meeting review already exists.");
			return;
		}

		DbMeetingSummary summary = new DbMeetingSummary
		{
			CreatedAt = DateTime.Now,
			MeetingId = meeting.Id,
			StarsCount = starsCount,
		};

		await getTeacherDbContext.AddAsync(summary);
		await getTeacherDbContext.SaveChangesAsync();
	}

	public async Task<DbMeeting?> GetMeeting(Guid guid)
	{
		return await getTeacherDbContext.Meetings
			.Where(l => l.Guid == guid)
				.Include(l => l.MeetingSummary)
				.Include(l => l.Student)
				.Include(l => l.Teacher)
				.Include(l => l.Subject)
			.FirstOrDefaultAsync();
	}

	public async Task<DbMeetingSummary?> GetMeetingSummary(Guid guid)
	{
		DbMeeting? meeting = await GetMeeting(guid);
		if (meeting is null)
			return null;

		return meeting.MeetingSummary;
	}

	public async Task<ICollection<DbMeeting>> GetAllStudentMeetings(DbStudent student)
	{
		return await getTeacherDbContext.Meetings
			.Where(l => l.StudentId == student.Id)
				.Include(l => l.Teacher)
				.Include(l => l.Subject)
			.ToListAsync();
	}

	public async Task<ICollection<DbMeeting>> GetAllTeacherMeetings(DbTeacher teacher)
	{
		return await getTeacherDbContext
			.Meetings
			.Where(l => l.Teacher.Id == teacher.Id)
				.Include(l => l.Student)
				.Include(l => l.Subject)
			.ToListAsync();
	}
}