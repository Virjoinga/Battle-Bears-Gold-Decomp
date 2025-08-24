namespace Analytics.Parameters
{
	public class MyTeamCountParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamCount;
			}
		}

		public MyTeamCountParameter(int amount)
			: base(amount)
		{
		}
	}
}
