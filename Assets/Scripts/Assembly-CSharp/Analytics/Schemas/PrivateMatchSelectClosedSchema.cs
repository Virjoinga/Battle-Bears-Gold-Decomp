namespace Analytics.Schemas
{
	public class PrivateMatchSelectClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privateMatchSelectClosed;
			}
		}
	}
}
