using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MatchmakingCompletedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchmakingCompleted;
			}
		}

		public MatchmakingCompletedSchema(MatchmakingWaitTimeParameter matchmakingWaitTime)
			: base(matchmakingWaitTime)
		{
		}
	}
}
