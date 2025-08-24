namespace Analytics.Parameters
{
	public class TransactionReceiptSignatureParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.transactionReceiptSignature;
			}
		}

		public TransactionReceiptSignatureParameter(string value)
			: base(value)
		{
		}
	}
}
