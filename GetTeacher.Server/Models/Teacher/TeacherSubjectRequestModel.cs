namespace GetTeacher.Server.Models.Teacher;

public class TeacherSubjectRequestModel
{
	public required ICollection<TeacherSubject> TeacherSubjects { get; set; }
}

public class TeacherSubject
{
	public required string Grade { get; set; }
	public required string Subject { get; set; }
}