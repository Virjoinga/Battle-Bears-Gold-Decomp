using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class RewardableActionClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.rewardableActionClicked;
			}
		}

		public RewardableActionClickedSchema(RewardableActionNameParameter rewardableActionName)
			: base(rewardableActionName)
		{
		}
	}
}
