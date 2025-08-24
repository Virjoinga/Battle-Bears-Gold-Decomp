using System.Collections.Generic;
using Analytics.Parameters;
using Analytics.Schemas;
using Constants;

namespace Analytics
{
	public class DealTransactionEventHelper : VirtualCurrencyTransactionEventHelper
	{
		public static TransactionSchema Transaction(Deal deal)
		{
			Stats stats = EventHelper.GetStats();
			return new TransactionSchema(new TransactionNameParameter(Constants.Transaction.BUY_DEAL), new TransactionTypeParameter(TransactionTypeParameter.Type.PURCHASE), new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level), new IsGuestAccountParameter(stats.guest), ItemsReceived(deal), null, null, null, null, new VirtualCurrencyParameters[1] { VirtualCurrencyCost(deal) }, new TransactionIDParameter(TransactionID(EventHelper.SessionId, deal.name)));
		}

		public static VirtualPurchaseSucceededSchema PurchaseSucceeded(Deal deal)
		{
			return new VirtualPurchaseSucceededSchema(new ItemParameters(new ItemAmountParameter(1), new ItemNameParameter(deal.name), new ItemTypeParameter(ItemTypeParameter.ItemType.DEAL)), VirtualCurrencyCost(deal));
		}

		public static VirtualPurchaseFailedSchema PurchaseFailed(Deal deal)
		{
			return new VirtualPurchaseFailedSchema(new ItemParameters(new ItemAmountParameter(1), new ItemNameParameter(deal.name), new ItemTypeParameter(ItemTypeParameter.ItemType.DEAL)), VirtualCurrencyCost(deal), new VirtualPurchaseErrorParameter(ServiceManager.Instance.LastError));
		}

		private static ItemParameters[] ItemsReceived(Deal deal)
		{
			List<ItemParameters> list = new List<ItemParameters>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (int item_id in deal.item_ids)
			{
				if (!dictionary.ContainsKey(item_id))
				{
					dictionary.Add(item_id, 1);
					continue;
				}
				Dictionary<int, int> dictionary2;
				Dictionary<int, int> dictionary3 = (dictionary2 = dictionary);
				int key;
				int key2 = (key = item_id);
				key = dictionary2[key];
				dictionary3[key2] = key + 1;
			}
			foreach (KeyValuePair<int, int> item2 in dictionary)
			{
				Item itemByID = ServiceManager.Instance.GetItemByID(item2.Key);
				ItemParameters item = VirtualCurrencyTransactionEventHelper.ItemReceived(itemByID);
				list.Add(item);
			}
			return list.ToArray();
		}

		private static VirtualCurrencyParameters VirtualCurrencyCost(Deal deal)
		{
			int? gas = deal.gas;
			bool hasValue = gas.HasValue;
			int? num = ((!hasValue) ? deal.joules : deal.gas);
			VirtualCurrencyNameParameter.CurrencyName currencyName = ((!hasValue) ? VirtualCurrencyNameParameter.CurrencyName.JOULES : VirtualCurrencyNameParameter.CurrencyName.GAS);
			return new VirtualCurrencyParameters(new VirtualCurrencyAmountParameter(num.Value), new VirtualCurrencyNameParameter(currencyName), new VirtualCurrencyTypeParameter(currencyName));
		}

		private static string TransactionID(string sessionId, string dealId)
		{
			return EventHelper.Hash(sessionId + dealId);
		}
	}
}
