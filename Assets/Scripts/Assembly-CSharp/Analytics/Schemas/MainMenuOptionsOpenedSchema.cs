namespace Analytics.Schemas
{
	public class MainMenuOptionsOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.mainMenuOptionsOpened;
			}
		}
	}
}
