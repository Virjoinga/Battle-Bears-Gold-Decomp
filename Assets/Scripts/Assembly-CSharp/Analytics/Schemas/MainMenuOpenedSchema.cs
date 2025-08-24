namespace Analytics.Schemas
{
	public class MainMenuOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.mainMenuOpened;
			}
		}
	}
}
