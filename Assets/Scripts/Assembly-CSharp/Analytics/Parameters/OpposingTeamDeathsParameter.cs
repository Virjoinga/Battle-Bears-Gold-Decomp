namespace Analytics.Parameters
{
	public class OpposingTeamDeathsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamDeaths;
			}
		}

		public OpposingTeamDeathsParameter(int amount)
			: base(amount)
		{
		}
	}
}
