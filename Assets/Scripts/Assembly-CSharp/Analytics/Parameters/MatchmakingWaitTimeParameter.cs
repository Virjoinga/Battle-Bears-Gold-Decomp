namespace Analytics.Parameters
{
	public class MatchmakingWaitTimeParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchmakingWaitTime;
			}
		}

		public MatchmakingWaitTimeParameter(int amount)
			: base(amount)
		{
		}
	}
}
