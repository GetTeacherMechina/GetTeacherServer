using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public interface ICreditCharger
{
	Task<bool> MeetingTransaction(DbStudent student, DbTeacher teacher, DbMeeting meeting);
}