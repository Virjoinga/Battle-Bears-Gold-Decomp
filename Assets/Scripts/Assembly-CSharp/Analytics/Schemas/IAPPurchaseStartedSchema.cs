using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class IAPPurchaseStartedSchema : IAPPurchaseBaseSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.iapPurchaseStarted;
			}
		}

		public IAPPurchaseStartedSchema(IsGuestAccountParameter isGuestAccount, ProductIDParameter productID, RealCurrencyParameters realCurrency, VirtualCurrencyParameters virtualCurrency = null, ItemParameters item = null)
			: base(isGuestAccount, productID, realCurrency, virtualCurrency, item)
		{
		}
	}
}
