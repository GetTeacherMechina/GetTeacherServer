namespace GetTeacher.Server.Models.Meeting;

public class MeetingResponseModel
{
	public Guid MeetingGuid { get; set; }
	public required string CompanionName { get; set; } = string.Empty;
}