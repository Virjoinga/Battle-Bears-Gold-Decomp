using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class VirtualPurchaseFailedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.virtualPurchaseFailed;
			}
		}

		public VirtualPurchaseFailedSchema(ItemParameters item, VirtualCurrencyParameters virtualCurrency, VirtualPurchaseErrorParameter virtualPurchaseError)
		{
			_parameters.AddRange(item);
			_parameters.AddRange(virtualCurrency);
			_parameters.Add(virtualPurchaseError);
		}
	}
}
