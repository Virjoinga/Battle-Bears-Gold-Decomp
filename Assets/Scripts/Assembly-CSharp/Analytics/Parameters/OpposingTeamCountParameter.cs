namespace Analytics.Parameters
{
	public class OpposingTeamCountParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamCount;
			}
		}

		public OpposingTeamCountParameter(int amount)
			: base(amount)
		{
		}
	}
}
