using GetTeacher.Server.Services.Database;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.ReadyManager;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class StudentReadyTeacherCountNotifier(GetTeacherDbContext getTeacherDbContext, ITeacherReadyManager teacherReadyManager, IWebSocketSystem webSocketSystem) : IStudentReadyTeacherCountNotifier
{
	private readonly GetTeacherDbContext getTeacherDbContext = getTeacherDbContext;
	private readonly ITeacherReadyManager teacherReadyManager = teacherReadyManager;
	private readonly IWebSocketSystem webSocketSystem = webSocketSystem;

	public async Task NotifyStudentsReadyTeachers()
	{
		ICollection<SubjectReadyTeachersDescriptor> readyTeachersDescriptors = await teacherReadyManager.GetReadyTeachersDescriptors();
		ICollection<Task<bool>> studentUserIds = [.. getTeacherDbContext.Students.Select(s => webSocketSystem.SendAsync(s.DbUserId, new { readyTeachers = readyTeachersDescriptors }, "ReadyTeachersUpdate"))];
		await Task.WhenAll([.. studentUserIds]);
	}
}