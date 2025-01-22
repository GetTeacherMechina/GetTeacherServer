namespace GetTeacher.Server.Models.MeetingHistory;

public class MeetingsHistoryModel
{
	public required ICollection<MeetingHistoryModel> History { get; set; }
}

public class MeetingHistoryModel
{
	public string? SubjectName { get; set; }
	public string? PrtnerName { get; set; }
	public string? StartTime { get; set; }
	public string? EndTime { get; set; }
}
