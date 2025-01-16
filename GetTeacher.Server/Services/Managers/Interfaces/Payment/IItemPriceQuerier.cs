namespace GetTeacher.Server.Services.Managers.Interfaces.Payment;

public record PaymentItemDescriptor(int ItemId, string Description, double CreditAmount, double PriceInDollars);

public enum ItemType
{
	Credits,
}

public interface IItemPriceQuerier
{
	public Task<double?> GetItemPriceInDollars(ItemType itemType, int itemId);

	public Task<ICollection<PaymentItemDescriptor>> GetCreditPricesInDollars();
}