namespace Analytics.Parameters
{
	public class TransactionServerParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.transactionServer;
			}
		}

		public TransactionServerParameter(string value)
			: base(value)
		{
		}
	}
}
