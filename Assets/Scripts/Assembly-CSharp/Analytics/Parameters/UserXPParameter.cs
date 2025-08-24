namespace Analytics.Parameters
{
	public class UserXPParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.userXP;
			}
		}

		public UserXPParameter(int amount)
			: base(amount)
		{
		}
	}
}
