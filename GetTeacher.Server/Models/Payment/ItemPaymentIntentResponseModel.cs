namespace GetTeacher.Server.Models.Payment;

public class ItemPaymentIntentResponseModel
{
	public string Status { get; set; } = string.Empty;

	public string ClientSecret { get; set; } = string.Empty;
}