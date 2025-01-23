using GetTeacher.Server.Models.Meeting;

namespace GetTeacher.Server.Models.CsGoContract;

public class CsGoContractRequestModel
{
	public bool IsFavorite { get; set; }

	public string TeacherBio { get; set; } = string.Empty;

	public double TeacherRank { get; set; }

	public double TeacherTariffPerMinute { get; set; }

	public required MeetingResponseModel MeetingResponseModel { get; set; }
}