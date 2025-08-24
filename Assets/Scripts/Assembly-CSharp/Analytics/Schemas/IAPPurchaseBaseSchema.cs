using Analytics.Parameters;

namespace Analytics.Schemas
{
	public abstract class IAPPurchaseBaseSchema : EventSchema
	{
		public IAPPurchaseBaseSchema(IsGuestAccountParameter isGuestAccount, ProductIDParameter productID, RealCurrencyParameters realCurrency, VirtualCurrencyParameters virtualCurrency = null, ItemParameters item = null)
		{
			_parameters.Add(isGuestAccount);
			_parameters.Add(productID);
			_parameters.AddRange(realCurrency);
			if (virtualCurrency != null)
			{
				_parameters.AddRange(virtualCurrency);
			}
			if (item != null)
			{
				_parameters.AddRange(item);
			}
		}
	}
}
