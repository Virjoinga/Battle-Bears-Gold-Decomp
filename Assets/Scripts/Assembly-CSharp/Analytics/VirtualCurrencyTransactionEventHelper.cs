using Analytics.Parameters;

namespace Analytics
{
	public abstract class VirtualCurrencyTransactionEventHelper : EventHelper
	{
		public static ItemParameters ItemReceived(Item item)
		{
			return new ItemParameters(new ItemAmountParameter(1), new ItemNameParameter(item.name), new ItemTypeParameter(item));
		}
	}
}
