namespace Analytics.Schemas
{
	public class LoginSucceededSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.loginSucceeded;
			}
		}
	}
}
