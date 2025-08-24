namespace Analytics.Schemas
{
	public class MainMenuOptionsClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.mainMenuOptionsClosed;
			}
		}
	}
}
