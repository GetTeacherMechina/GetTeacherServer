using GetTeacher.Server.Models.Meeting;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;

namespace GetTeacher.Server.Services.Managers.Implementations;

public class StudentMatcher(IMeetingMatcher meetingMatcher, IWebSocketHandler webSocketHandler) : IStudentMatcher
{
	private readonly IMeetingMatcher meetingMatcher = meetingMatcher;

	public async Task MatchLoop(DbStudent student, DbSubject subject)
	{
		// TODO: Poll every time the teacher's status is changed using a cancellation token
		DbTeacher? foundTeacher = null;
		while (foundTeacher is null)
		{
			await Task.Delay(500); // Nice
			foundTeacher = await meetingMatcher.MatchStudentTeacher(student, subject);
		}

		// Signal student and teacher :)
		try
		{
			Task sendTeacher = webSocketHandler.SendAsync(foundTeacher.DbUser.Id, new MeetingResponse { MatchedWith = student.DbUser.UserName! });
			Task sendStudent = webSocketHandler.SendAsync(student.DbUser.Id, new MeetingResponse { MatchedWith = foundTeacher.DbUser.UserName! });
			await Task.WhenAll(sendTeacher, sendStudent);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}