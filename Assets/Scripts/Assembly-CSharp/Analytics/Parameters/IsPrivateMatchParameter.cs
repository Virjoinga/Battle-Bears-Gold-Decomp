namespace Analytics.Parameters
{
	public class IsPrivateMatchParameter : BooleanParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.isPrivateMatch;
			}
		}

		public IsPrivateMatchParameter(bool isPrivateMatch)
			: base(isPrivateMatch)
		{
		}
	}
}
