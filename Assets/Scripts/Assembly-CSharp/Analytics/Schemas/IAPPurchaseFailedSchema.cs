using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class IAPPurchaseFailedSchema : IAPPurchaseBaseSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.iapPurchaseFailed;
			}
		}

		public IAPPurchaseFailedSchema(IAPPurchaseErrorParameter iapPurchaseError, IsGuestAccountParameter isGuestAccount, ProductIDParameter productID, RealCurrencyParameters realCurrency, VirtualCurrencyParameters virtualCurrency = null, ItemParameters item = null)
			: base(isGuestAccount, productID, realCurrency, virtualCurrency, item)
		{
			_parameters.Add(iapPurchaseError);
		}
	}
}
