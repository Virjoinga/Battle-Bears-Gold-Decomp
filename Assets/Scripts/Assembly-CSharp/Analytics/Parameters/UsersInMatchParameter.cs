namespace Analytics.Parameters
{
	public class UsersInMatchParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.usersInMatch;
			}
		}

		public UsersInMatchParameter(int amount)
			: base(amount)
		{
		}
	}
}
