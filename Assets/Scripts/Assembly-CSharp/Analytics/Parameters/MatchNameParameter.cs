namespace Analytics.Parameters
{
	public class MatchNameParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchName;
			}
		}

		public MatchNameParameter(string value)
			: base(value)
		{
		}
	}
}
