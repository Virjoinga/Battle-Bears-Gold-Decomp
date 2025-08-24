using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class IAPPurchaseValidatedSchema : IAPPurchaseBaseSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.iapPurchaseValidated;
			}
		}

		public IAPPurchaseValidatedSchema(TransactionIDParameter transactionID, IsGuestAccountParameter isGuestAccount, ProductIDParameter productID, RealCurrencyParameters realCurrency, VirtualCurrencyParameters virtualCurrency = null, ItemParameters item = null)
			: base(isGuestAccount, productID, realCurrency, virtualCurrency, item)
		{
			_parameters.Add(transactionID);
		}
	}
}
