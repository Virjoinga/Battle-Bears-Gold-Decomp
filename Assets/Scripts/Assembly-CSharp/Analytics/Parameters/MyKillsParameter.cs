namespace Analytics.Parameters
{
	public class MyKillsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myKills;
			}
		}

		public MyKillsParameter(int amount)
			: base(amount)
		{
		}
	}
}
