namespace Analytics.Parameters
{
	public class MyDeathsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myDeaths;
			}
		}

		public MyDeathsParameter(int amount)
			: base(amount)
		{
		}
	}
}
