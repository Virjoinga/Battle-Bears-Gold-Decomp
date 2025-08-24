using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class RewardGrantFailedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.rewardGrantFailed;
			}
		}

		public RewardGrantFailedSchema(RewardableActionNameParameter rewardableActionName, RewardGrantErrorParameter rewardGrantError)
			: base(rewardableActionName, rewardGrantError)
		{
		}
	}
}
