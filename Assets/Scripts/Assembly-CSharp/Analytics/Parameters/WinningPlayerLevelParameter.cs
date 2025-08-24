namespace Analytics.Parameters
{
	public class WinningPlayerLevelParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.winningPlayerLevel;
			}
		}

		public WinningPlayerLevelParameter(int amount)
			: base(amount)
		{
		}
	}
}
