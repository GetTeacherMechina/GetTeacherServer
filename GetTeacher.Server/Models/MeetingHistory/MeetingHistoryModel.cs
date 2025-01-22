namespace GetTeacher.Server.Models.MeetingHistory;

public class MeetingsHistoryModel
{
	public required ICollection<MeetingHistoryModel> History { get; set; }
}

public class MeetingHistoryModel
{
	public string SubjectName { get; set; } = string.Empty;

	public string PartnerName { get; set; } = string.Empty;

	public DateTime StartTime { get; set; }

	public DateTime EndTime { get; set; }
}
