namespace GetTeacher.Server.Models.Meeting;

[Serializable]
public class MeetingResponse
{
	public int CallId { get; set; }
	public string MatchedWith { get; set; } = string.Empty;
}