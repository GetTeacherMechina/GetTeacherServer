namespace GetTeacher.Server.Models.Authentication.Register;

public class VerifyEmail2FaRequestModel
{
	public required string Email { get; set; }

	public required string Code { get; set; }
}