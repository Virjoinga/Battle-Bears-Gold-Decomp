using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class IAPPurchaseInvalidatedSchema : IAPPurchaseBaseSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.iapPurchaseInvalidated;
			}
		}

		public IAPPurchaseInvalidatedSchema(TransactionIDParameter transactionID, TransactionReceiptParameter transactionReceipt, TransactionReceiptSignatureParameter transactionReceiptSignature, IsGuestAccountParameter isGuestAccount, ProductIDParameter productID, RealCurrencyParameters realCurrency, VirtualCurrencyParameters virtualCurrency = null, ItemParameters item = null)
			: base(isGuestAccount, productID, realCurrency, virtualCurrency, item)
		{
			_parameters.Add(transactionID);
			_parameters.Add(transactionReceipt);
			_parameters.Add(transactionReceiptSignature);
		}
	}
}
