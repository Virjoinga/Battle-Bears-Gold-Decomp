namespace Analytics.Parameters
{
	public class TransactionNameParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.transactionName;
			}
		}

		public TransactionNameParameter(string value)
			: base(value)
		{
		}
	}
}
