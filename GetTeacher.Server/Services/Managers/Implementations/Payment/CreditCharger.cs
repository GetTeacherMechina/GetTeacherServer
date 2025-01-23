using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;

namespace GetTeacher.Server.Services.Managers.Implementations.Payment;

public class CreditCharger(IUserCreditManager userCreditManager, IMeetingManager meetingManager) : ICreditCharger
{
	private readonly IUserCreditManager userCreditManager = userCreditManager;
	private readonly IMeetingManager meetingManager = meetingManager;

	public async Task<bool> MeetingTransaction(DbStudent student, DbTeacher teacher, DbMeeting meeting)
	{
		TimeSpan? meetingLength = await meetingManager.GetMeetingLength(meeting.Guid);
		if (meetingLength is null)
			return false;

		double credits = meeting.Teacher.TariffPerMinute * meetingLength.Value.TotalMinutes;
		await userCreditManager.RemoveCreditsFromUser(student.DbUser, credits);
		await userCreditManager.AddCreditsToUser(teacher.DbUser, credits);
		return true;
	}
}