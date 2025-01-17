namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public record PaymentDetailsModel(double DollarAmount);
public record PaymentResultModel(bool Success, double DollarAmount);
public record PaymentIntendDescriptor(string PaymentIntendId, string ClientSecret);

public interface IPaymentManager
{
	public Task<PaymentIntendDescriptor?> MakePaymentIntentAsync(PaymentDetailsModel paymentDetailsModel);

	public Task<PaymentResultModel> ConfirmPaymentIntentAsync(string clientSecret);
}