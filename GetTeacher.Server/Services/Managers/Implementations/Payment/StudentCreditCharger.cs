using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;

namespace GetTeacher.Server.Services.Managers.Implementations.Payment;

public class StudentCreditCharger(IUserCreditManager userCreditManager, IMeetingManager meetingManager) : IStudentCreditCharger
{
	private readonly IUserCreditManager userCreditManager = userCreditManager;
	private readonly IMeetingManager meetingManager = meetingManager;

	public async Task<bool> ChargeStudent(DbStudent student, DbMeeting meeting)
	{
		TimeSpan? meetingLength = await meetingManager.GetMeetingLength(meeting.Guid);
		if (meetingLength is null)
			return false;

		await userCreditManager.RemoveCreditsFromUser(student.DbUser, meeting.Teacher.TariffPerMinute * meetingLength.Value.TotalMinutes);
		return true;
	}
}