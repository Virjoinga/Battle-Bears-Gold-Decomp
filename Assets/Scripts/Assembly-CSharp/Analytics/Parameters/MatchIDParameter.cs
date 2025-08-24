namespace Analytics.Parameters
{
	public class MatchIDParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchID;
			}
		}

		public MatchIDParameter(string value)
			: base(value)
		{
		}
	}
}
