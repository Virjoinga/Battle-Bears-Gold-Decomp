namespace Analytics.Schemas
{
	public class PrivacyClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privacyClicked;
			}
		}
	}
}
