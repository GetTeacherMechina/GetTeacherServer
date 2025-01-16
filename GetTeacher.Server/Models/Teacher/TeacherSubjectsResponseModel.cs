using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Models.Teacher;

public class TeacherSubjectsResponseModel
{
	public ICollection<DbTeacherSubject> TeacherSubjects { get; set; } = [];
}