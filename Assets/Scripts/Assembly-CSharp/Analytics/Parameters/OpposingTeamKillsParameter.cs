namespace Analytics.Parameters
{
	public class OpposingTeamKillsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamKills;
			}
		}

		public OpposingTeamKillsParameter(int amount)
			: base(amount)
		{
		}
	}
}
