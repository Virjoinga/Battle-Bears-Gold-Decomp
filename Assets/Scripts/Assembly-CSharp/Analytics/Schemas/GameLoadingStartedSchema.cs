namespace Analytics.Schemas
{
	public class GameLoadingStartedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.gameLoadingStarted;
			}
		}
	}
}
