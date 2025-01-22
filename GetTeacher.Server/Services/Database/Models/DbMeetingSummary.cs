using System.ComponentModel.DataAnnotations.Schema;

namespace GetTeacher.Server.Services.Database.Models;

public class DbMeetingSummary
{
	public int Id { get; set; }

	[ForeignKey(nameof(Meeting))]
	public int MeetingId { get; set; }
	public virtual DbMeeting Meeting { get; set; } = null!;

	public int StarsCount { get; set; }

	public DateTime CreatedAt { get; set; }
}