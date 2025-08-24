namespace Analytics.Parameters
{
	public class OpposingTeamBombsDepositedParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.opposingTeamBombsDeposited;
			}
		}

		public OpposingTeamBombsDepositedParameter(int amount)
			: base(amount)
		{
		}
	}
}
