using System.Collections.Concurrent;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;

namespace GetTeacher.Server.Services.Managers.Implementations.Payment;

public class LocalPaymentIntentToCredits : IPaymentIntentToCredits
{
	private static readonly ConcurrentDictionary<string, double> paymentIntentIdToCreditAmount = [];

	public Task Push(string paymentIntentId, double creditAmount)
	{
		paymentIntentIdToCreditAmount.TryAdd(paymentIntentId, creditAmount);
		return Task.CompletedTask;
	}

	public Task<double?> Pop(string paymentIntentId)
	{
		if (paymentIntentIdToCreditAmount.Remove(paymentIntentId, out double creditAmount))
			return Task.FromResult((double?)creditAmount);

		return Task.FromResult<double?>(null);
	}
}