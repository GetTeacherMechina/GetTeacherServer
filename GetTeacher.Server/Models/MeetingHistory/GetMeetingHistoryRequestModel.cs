namespace GetTeacher.Server.Models.MeetingHistory;

public class GetMeetingHistoryRequestModel
{
	public required bool IsTeacher { get; set; }
	public required bool IsStudent { get; set; }
}