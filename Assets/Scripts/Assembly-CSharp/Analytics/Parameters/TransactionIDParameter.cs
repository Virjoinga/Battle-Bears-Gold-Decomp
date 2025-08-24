namespace Analytics.Parameters
{
	public class TransactionIDParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.transactionID;
			}
		}

		public TransactionIDParameter(string value)
			: base(value)
		{
		}
	}
}
