namespace Analytics.Parameters
{
	public class WinningPlayerKillsParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.winningPlayerKills;
			}
		}

		public WinningPlayerKillsParameter(int amount)
			: base(amount)
		{
		}
	}
}
