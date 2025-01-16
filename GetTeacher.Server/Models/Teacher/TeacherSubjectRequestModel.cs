using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Models.Teacher;

public class TeacherSubjectRequestModel
{
	public required string Subject { get; set; }
	public required string Grade { get; set; }
}