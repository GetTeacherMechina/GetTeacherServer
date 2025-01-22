using GetTeacher.Server.Services.Database.Models;

namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public interface IStudentCreditCharger
{
	Task<bool> ChargeStudent(DbStudent student, DbMeeting meeting);
}