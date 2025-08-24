namespace Analytics.Parameters
{
	public class KillerLevelParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.killerLevel;
			}
		}

		public KillerLevelParameter(int amount)
			: base(amount)
		{
		}
	}
}
