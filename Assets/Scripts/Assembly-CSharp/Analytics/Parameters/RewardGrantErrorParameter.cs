namespace Analytics.Parameters
{
	public class RewardGrantErrorParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.rewardGrantError;
			}
		}

		public RewardGrantErrorParameter(string value)
			: base(value)
		{
		}
	}
}
