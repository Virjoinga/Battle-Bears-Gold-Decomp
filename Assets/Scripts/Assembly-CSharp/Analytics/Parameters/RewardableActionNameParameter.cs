namespace Analytics.Parameters
{
	public class RewardableActionNameParameter : StringParameter
	{
		public enum Action
		{
			BATTLEBEARS_COM = 0,
			FACEBOOK = 1,
			OTHER_APP = 2,
			TWITTER = 3,
			YOUTUBE = 4
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.rewardableActionName;
			}
		}

		public RewardableActionNameParameter(Action action)
			: base(action.ToString())
		{
		}
	}
}
