namespace Analytics.Parameters
{
	public class OppositionAverageLevelParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.oppositionAverageLevel;
			}
		}

		public OppositionAverageLevelParameter(int amount)
			: base(amount)
		{
		}
	}
}
