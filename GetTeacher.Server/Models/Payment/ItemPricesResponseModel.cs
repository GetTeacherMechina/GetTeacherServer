using GetTeacher.Server.Services.Managers.Interfaces.Payment;

namespace GetTeacher.Server.Models.Payment;

public class ItemPricesResponseModel
{
	public ICollection<PaymentItemDescriptor> CreditPrices { get; set; } = [];
}