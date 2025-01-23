namespace GetTeacher.Server.Models.ReportTeacher;

public class ReportTeacherRequestModel
{
	public required Guid MeetingGuid { get; set; }

	public required string ReportContent {  get; set; }
}