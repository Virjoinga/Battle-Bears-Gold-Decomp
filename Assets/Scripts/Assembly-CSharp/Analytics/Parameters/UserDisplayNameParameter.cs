namespace Analytics.Parameters
{
	public class UserDisplayNameParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.userDisplayName;
			}
		}

		public UserDisplayNameParameter(string value)
			: base(value)
		{
		}
	}
}
