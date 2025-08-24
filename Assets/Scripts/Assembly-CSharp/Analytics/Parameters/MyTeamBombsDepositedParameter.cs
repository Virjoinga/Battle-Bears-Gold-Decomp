namespace Analytics.Parameters
{
	public class MyTeamBombsDepositedParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.myTeamBombsDeposited;
			}
		}

		public MyTeamBombsDepositedParameter(int amount)
			: base(amount)
		{
		}
	}
}
