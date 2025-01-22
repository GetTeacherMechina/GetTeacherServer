using GetTeacher.Server.Services.Managers.Interfaces.Payment;

namespace GetTeacher.Server.Models.Payment;

public class BuyItemRequestModel
{
	public required PaymentItemDescriptor Item { get; set; }
}