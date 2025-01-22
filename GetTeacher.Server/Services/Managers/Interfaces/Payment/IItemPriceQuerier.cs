namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public record PaymentItemDescriptor(int ItemId, string Description, double Amount, double PriceInDollars);

public enum ItemType
{
	Credits,
}

public interface IItemPriceQuerier
{
	public Task<PaymentItemDescriptor?> GetItem(ItemType itemType, int itemId);

	public Task<ICollection<PaymentItemDescriptor>> GetCreditPricesInDollars();
}