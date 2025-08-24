namespace Analytics.Parameters
{
	public class WinningPlayerDeathsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.winningPlayerDeaths;
			}
		}

		public WinningPlayerDeathsParameter(int amount)
			: base(amount)
		{
		}
	}
}
