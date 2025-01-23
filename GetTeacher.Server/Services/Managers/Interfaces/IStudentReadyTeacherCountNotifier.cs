using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces;

public interface IStudentReadyTeacherCountNotifier
{
	Task NotifyStudentsReadyTeachers();
}