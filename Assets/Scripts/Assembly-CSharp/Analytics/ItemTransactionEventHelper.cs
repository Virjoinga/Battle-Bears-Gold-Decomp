using Analytics.Parameters;
using Analytics.Schemas;
using Constants;

namespace Analytics
{
	public class ItemTransactionEventHelper : VirtualCurrencyTransactionEventHelper
	{
		public static TransactionSchema Transaction(Item item, Purchaseable purchaseable)
		{
			Stats stats = EventHelper.GetStats();
			return new TransactionSchema(new TransactionNameParameter(Constants.Transaction.BUY_ITEM_PREFIX + item.type), new TransactionTypeParameter(TransactionTypeParameter.Type.PURCHASE), new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level), new IsGuestAccountParameter(stats.guest), new ItemParameters[1] { VirtualCurrencyTransactionEventHelper.ItemReceived(item) }, null, null, null, null, new VirtualCurrencyParameters[1] { VirtualCurrencyCost(purchaseable) }, new TransactionIDParameter(TransactionID(item)));
		}

		public static VirtualPurchaseSucceededSchema PurchaseSucceeded(Item item, Purchaseable purchaseable)
		{
			return new VirtualPurchaseSucceededSchema(VirtualCurrencyTransactionEventHelper.ItemReceived(item), VirtualCurrencyCost(purchaseable));
		}

		public static VirtualPurchaseFailedSchema PurchaseFailed(Item item, Purchaseable purchaseable, string error)
		{
			return new VirtualPurchaseFailedSchema(VirtualCurrencyTransactionEventHelper.ItemReceived(item), VirtualCurrencyCost(purchaseable), new VirtualPurchaseErrorParameter(error));
		}

		private static VirtualCurrencyParameters VirtualCurrencyCost(Purchaseable purchaseable)
		{
			bool flag = purchaseable.current_gas > 0;
			VirtualCurrencyNameParameter.CurrencyName currencyName = ((!flag) ? VirtualCurrencyNameParameter.CurrencyName.JOULES : VirtualCurrencyNameParameter.CurrencyName.GAS);
			return new VirtualCurrencyParameters(new VirtualCurrencyAmountParameter((!flag) ? purchaseable.current_joules : purchaseable.current_gas), new VirtualCurrencyNameParameter(currencyName), new VirtualCurrencyTypeParameter(currencyName));
		}

		private static string TransactionID(Item item)
		{
			return EventHelper.Hash(EventHelper.SessionId + item.name);
		}
	}
}
