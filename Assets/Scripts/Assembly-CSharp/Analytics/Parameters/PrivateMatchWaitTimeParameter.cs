namespace Analytics.Parameters
{
	public class PrivateMatchWaitTimeParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.privateMatchWaitTime;
			}
		}

		public PrivateMatchWaitTimeParameter(int amount)
			: base(amount)
		{
		}
	}
}
