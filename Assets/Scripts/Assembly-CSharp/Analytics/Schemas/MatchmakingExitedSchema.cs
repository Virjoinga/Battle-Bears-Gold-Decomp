using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MatchmakingExitedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchmakingExited;
			}
		}

		public MatchmakingExitedSchema(MatchmakingWaitTimeParameter matchmakingWaitTime)
			: base(matchmakingWaitTime)
		{
		}
	}
}
