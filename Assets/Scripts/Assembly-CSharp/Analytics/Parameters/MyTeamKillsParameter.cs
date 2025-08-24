namespace Analytics.Parameters
{
	public class MyTeamKillsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamKills;
			}
		}

		public MyTeamKillsParameter(int amount)
			: base(amount)
		{
		}
	}
}
