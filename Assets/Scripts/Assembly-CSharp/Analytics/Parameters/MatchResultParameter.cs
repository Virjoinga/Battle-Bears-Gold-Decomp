namespace Analytics.Parameters
{
	public class MatchResultParameter : StringParameter
	{
		public enum Result
		{
			WIN = 0,
			LOSE = 1,
			DRAW = 2
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchResult;
			}
		}

		public MatchResultParameter(Result result)
			: base(result.ToString())
		{
		}
	}
}
