namespace GetTeacher.Server.Models.Profile;

public class ProfileResponseModel
{
	public string Result { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string FullName { get; set; } = string.Empty;

	public bool IsTeacher { get; set; } = false;

	public bool IsStudent { get; set; } = false;

	public double Credits { get; set; }
}