namespace Analytics.Parameters
{
	public class MyTeamLevelParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamLevel;
			}
		}

		public MyTeamLevelParameter(int amount)
			: base(amount)
		{
		}
	}
}
