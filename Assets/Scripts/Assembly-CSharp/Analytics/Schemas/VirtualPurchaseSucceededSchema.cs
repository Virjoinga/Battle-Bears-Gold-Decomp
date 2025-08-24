using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class VirtualPurchaseSucceededSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.virtualPurchaseSucceeded;
			}
		}

		public VirtualPurchaseSucceededSchema(ItemParameters item, VirtualCurrencyParameters virtualCurrency)
		{
			_parameters.AddRange(item);
			_parameters.AddRange(virtualCurrency);
		}
	}
}
