namespace GetTeacher.Server.Models.Meeting;

[Serializable]
public class MeetingResponse
{
	public int MeetingId { get; set; }
	public string CompanionName { get; set; } = string.Empty;
}