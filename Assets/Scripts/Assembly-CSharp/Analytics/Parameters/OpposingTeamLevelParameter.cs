namespace Analytics.Parameters
{
	public class OpposingTeamLevelParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamLevel;
			}
		}

		public OpposingTeamLevelParameter(int amount)
			: base(amount)
		{
		}
	}
}
