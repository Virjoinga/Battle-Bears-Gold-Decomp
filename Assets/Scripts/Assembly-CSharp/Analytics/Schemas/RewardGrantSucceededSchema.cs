using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class RewardGrantSucceededSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.rewardGrantSucceeded;
			}
		}

		public RewardGrantSucceededSchema(RewardableActionNameParameter rewardableActionName, VirtualCurrencyParameters virtualCurrency)
		{
			_parameters.Add(rewardableActionName);
			_parameters.AddRange(virtualCurrency);
		}
	}
}
