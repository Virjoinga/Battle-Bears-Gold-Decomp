using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PurchaseAdditionalFundsOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.purchaseAdditionalFundsOpened;
			}
		}

		public PurchaseAdditionalFundsOpenedSchema(ItemNameParameter itemName, ItemTypeParameter itemType, ProductIDParameter productID, RealCurrencyParameters realCurrency, VirtualCurrencyParameters virtualCurrency)
		{
			_parameters.Add(itemName);
			_parameters.Add(itemType);
			_parameters.Add(productID);
			_parameters.AddRange(realCurrency);
			_parameters.AddRange(virtualCurrency);
		}
	}
}
