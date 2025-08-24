namespace Analytics.Schemas
{
	public class MatchmakingEnteredSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchmakingEntered;
			}
		}
	}
}
