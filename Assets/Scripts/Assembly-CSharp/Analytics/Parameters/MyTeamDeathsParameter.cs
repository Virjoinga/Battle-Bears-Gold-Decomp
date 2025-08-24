namespace Analytics.Parameters
{
	public class MyTeamDeathsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamDeaths;
			}
		}

		public MyTeamDeathsParameter(int amount)
			: base(amount)
		{
		}
	}
}
