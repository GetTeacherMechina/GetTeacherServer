namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public record PaymentDetailsModel(double DollarAmount);
public record PaymentResultModel(bool Success, double DollarAmount);

public interface IPaymentManager
{
	public Task<string?> MakePaymentIntentAsync(PaymentDetailsModel paymentDetailsModel);

	public Task<PaymentResultModel> ConfirmPaymentIntentAsync(string clientSecret);
}