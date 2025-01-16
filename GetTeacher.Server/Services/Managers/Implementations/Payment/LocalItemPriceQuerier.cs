using GetTeacher.Server.Services.Managers.Interfaces.Payment;

namespace GetTeacher.Server.Services.Managers.Implementations.Payment;

public class LocalItemPriceQuerier : IItemPriceQuerier
{
	private static readonly IDictionary<int, PaymentItemDescriptor> creditPriceDescriptors = new Dictionary<int, PaymentItemDescriptor>
	{
		{ 1, new PaymentItemDescriptor(1, "Meh", 10, 10) },
		{ 2, new PaymentItemDescriptor(2, "Buy me", 50, 40) },
		{ 3, new PaymentItemDescriptor(3, "Wow this is so worth it", 100, 50) },
		{ 4, new PaymentItemDescriptor(4, "Cool kids will buy this ->", 1000, 100) },
	};

	public Task<double?> GetItemPriceInDollars(ItemType itemType, int itemId)
	{
		switch (itemType)
		{
			case ItemType.Credits:
				if (creditPriceDescriptors.TryGetValue(itemId, out PaymentItemDescriptor? value))
					return Task.FromResult((double?)value.PriceInDollars);
				return Task.FromResult<double?>(null);
		}

		return Task.FromResult<double?>(null);
	}

	public Task<ICollection<PaymentItemDescriptor>> GetCreditPricesInDollars()
	{
		return Task.FromResult(creditPriceDescriptors.Values);
	}
}