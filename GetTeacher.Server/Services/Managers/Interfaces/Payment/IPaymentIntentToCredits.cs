namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public interface IPaymentIntentToCredits
{
	Task Push(string paymentIntentId, double creditAmount);

	Task<double?> Pop(string paymentIntentId);
}