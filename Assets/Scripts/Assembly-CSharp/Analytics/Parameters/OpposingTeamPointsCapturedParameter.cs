namespace Analytics.Parameters
{
	public class OpposingTeamPointsCapturedParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamPointsCaptured;
			}
		}

		public OpposingTeamPointsCapturedParameter(int amount)
			: base(amount)
		{
		}
	}
}
