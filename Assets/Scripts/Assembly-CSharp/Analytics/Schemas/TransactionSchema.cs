using System.Collections.Generic;
using Analytics.Parameters;
using Analytics.Parameters.Collections;

namespace Analytics.Schemas
{
	public class TransactionSchema : EventSchema
	{
		private Dictionary<string, object> _dict;

		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.transaction;
			}
		}

		public TransactionSchema(TransactionNameParameter transactionName, TransactionTypeParameter transactionType, UserSkillParameter userSkill, UserLevelParameter userLevel, IsGuestAccountParameter isGuestAccount, ItemParameters[] itemsReceived = null, RealCurrencyParameters realCurrencyReceived = null, VirtualCurrencyParameters[] virtualCurrenciesReceived = null, ItemParameters[] itemsSpent = null, RealCurrencyParameters realCurrencySpent = null, VirtualCurrencyParameters[] virtualCurrenciesSpent = null, TransactionIDParameter transactionID = null, ProductIDParameter productID = null, TransactionReceiptSignatureParameter transactionReceiptSignature = null, TransactionReceiptParameter transactionReceipt = null)
		{
			_dict = new Dictionary<string, object>();
			AddToDict(transactionName);
			AddToDict(transactionType);
			AddToDict(userSkill);
			AddToDict(userLevel);
			AddToDict(isGuestAccount);
			AddToDict("productsReceived", itemsReceived, realCurrencyReceived, virtualCurrenciesReceived);
			AddToDict("productsSpent", itemsSpent, realCurrencySpent, virtualCurrenciesSpent);
			if (productID != null)
			{
				AddToDict(productID);
			}
			if (transactionID != null)
			{
				AddToDict(transactionID);
			}
			AddToDict(new TransactionServerParameter("GOOGLE"));
			if (transactionReceiptSignature != null)
			{
				AddToDict(transactionReceiptSignature);
			}
			if (transactionReceipt != null)
			{
				AddToDict(transactionReceipt);
			}
		}

		private void AddToDict(string name, ItemParameters[] itemParameters, RealCurrencyParameters realCurrencyParameters, VirtualCurrencyParameters[] virtualCurrencyParameters)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (itemParameters != null)
			{
				List<object> list = new List<object>();
				foreach (ItemParameters itemParameters2 in itemParameters)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					foreach (IEventParameter item in itemParameters2)
					{
						AddToDict(item, dictionary2);
					}
					list.Add(new Dictionary<string, object> { { "item", dictionary2 } });
				}
				dictionary.Add("items", list.ToArray());
			}
			if (realCurrencyParameters != null)
			{
				Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
				AddToDict(realCurrencyParameters, dictionary3);
				dictionary.Add("realCurrency", dictionary3);
			}
			if (virtualCurrencyParameters != null)
			{
				List<object> list2 = new List<object>();
				foreach (VirtualCurrencyParameters virtualCurrencyParameters2 in virtualCurrencyParameters)
				{
					Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
					foreach (IEventParameter item2 in virtualCurrencyParameters2)
					{
						AddToDict(item2, dictionary4);
					}
					list2.Add(new Dictionary<string, object> { { "virtualCurrency", dictionary4 } });
				}
				dictionary.Add("virtualCurrencies", list2.ToArray());
			}
			_dict.Add(name, dictionary);
		}

		private void AddToDict(IEventParameterEnumerable parameterEnumerable, Dictionary<string, object> dict = null)
		{
			foreach (IEventParameter item in parameterEnumerable)
			{
				AddToDict(item, dict);
			}
		}

		private void AddToDict(IEventParameter eventParameter, Dictionary<string, object> dict = null)
		{
			(dict ?? _dict).Add(eventParameter.Name, eventParameter.Value);
		}

		public override IDictionary<string, object> ToDictionary()
		{
			return _dict;
		}
	}
}
