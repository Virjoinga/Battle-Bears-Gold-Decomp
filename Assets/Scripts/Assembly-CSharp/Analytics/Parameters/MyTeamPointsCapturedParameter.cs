namespace Analytics.Parameters
{
	public class MyTeamPointsCapturedParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamPointsCaptured;
			}
		}

		public MyTeamPointsCapturedParameter(int amount)
			: base(amount)
		{
		}
	}
}
