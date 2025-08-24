namespace Analytics.Parameters
{
	public class UserLevelParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.userLevel;
			}
		}

		public UserLevelParameter(double amount)
			: this((int)amount)
		{
		}

		public UserLevelParameter(int amount)
			: base(amount)
		{
		}
	}
}
