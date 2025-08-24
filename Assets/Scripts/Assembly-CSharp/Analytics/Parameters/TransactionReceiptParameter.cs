namespace Analytics.Parameters
{
	public class TransactionReceiptParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.transactionReceipt;
			}
		}

		public TransactionReceiptParameter(string value)
			: base(value)
		{
		}
	}
}
