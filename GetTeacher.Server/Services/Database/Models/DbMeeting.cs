using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;

public class DbMeeting
{
	public int Id { get; set; }

	public Guid Guid { get; set; }

	public DateTime StartTime { get; set; }

	[ForeignKey(nameof(Teacher))]
	public int TeacherId { get; set; }
	public virtual DbTeacher Teacher { get; set; } = null!;

	[ForeignKey(nameof(Student))]
	public int StudentId { get; set; }
	public virtual DbStudent Student { get; set; } = null!;

	[ForeignKey(nameof(Subject))]
	public int SubjectId { get; set; }
	public DbSubject Subject { get; set; } = null!;

	[ForeignKey(nameof(Grade))]
	public int GradeId { get; set; }
	public DbGrade Grade { get; set; } = null!;

	[ForeignKey(nameof(MeetingSummary))]
	public int MeetingSummaryId { get; set; }
	public DbMeetingSummary? MeetingSummary { get; set; } = null!;
}
