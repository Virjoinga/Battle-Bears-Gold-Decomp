namespace Analytics.Schemas
{
	public class LoginMenuOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.loginMenuOpened;
			}
		}
	}
}
