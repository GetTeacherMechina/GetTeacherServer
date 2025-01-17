using GetTeacher.Server.Services.Managers.Interfaces.Payment;
using Stripe;

namespace GetTeacher.Server.Services.Managers.Implementations.Payment;

public class StripePaymentManager(IPaymentIntentToCredits paymentIntentToCredits, ILogger<IPaymentManager> logger, IConfiguration configuration) : IPaymentManager
{
	private readonly ILogger<IPaymentManager> logger = logger;
	private readonly IConfiguration configuration = configuration;

	public async Task<PaymentIntendDescriptor?> MakePaymentIntentAsync(PaymentDetailsModel paymentDetailsModel)
	{
		string? stripeApiKey = configuration["StripeSettings:ApiKey"];
		if (stripeApiKey is null)
		{
			logger.LogCritical("StripeSettings:ApiKey not set.");
			return null;
		}
		StripeConfiguration.StripeClient = new StripeClient(stripeApiKey);

		try
		{

			// Create a PaymentIntent
			PaymentIntentService paymentIntentService = new PaymentIntentService();
			PaymentIntent paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
			{
				Amount = (long)(paymentDetailsModel.DollarAmount * 100), // From dollars to cents
				Currency = "USD",
				PaymentMethod = "pm_card_visa",
				Description = "Get Teacher == Cash Grab",
			});

			logger.LogCritical("Payment Intent Id: {paymentIntentId}", paymentIntent.Id);
			return new PaymentIntendDescriptor(paymentIntent.Id, paymentIntent.ClientSecret);
		}
		catch (StripeException ex)
		{
			logger.LogError(ex, "Stripe error.");
			return null;
		}
	}

	public async Task<PaymentResultModel> ConfirmPaymentIntentAsync(string paymentIntentId)
	{
		string? stripeApiKey = configuration["StripeSettings:ApiKey"];
		if (stripeApiKey is null)
		{
			logger.LogCritical("StripeSettings:ApiKey not set.");
			return new PaymentResultModel(false, 0);
		}
		StripeConfiguration.StripeClient = new StripeClient(stripeApiKey);

		var paymentIntentService = new PaymentIntentService();

		while (true)
		{
			PaymentIntent paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);
			
			if (paymentIntent.Status == "succeeded")
			{
				Console.WriteLine("Payment succeeded!");
				return new PaymentResultModel(true, paymentIntent.Amount / 100);
			}

			if (paymentIntent.Status == "requires_action" || paymentIntent.Status == "requires_payment_method")
			{
				logger.LogWarning("Stripe payment requires extra action(s).");
				return new PaymentResultModel(false, 0);
			}

			logger.LogInformation("Current payment status: {paymentIntentStatus}", paymentIntent.Status);
			await Task.Delay(1000);
		}
	}
}