namespace Analytics.Schemas
{
	public class InGameOptionsClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.inGameOptionsClosed;
			}
		}
	}
}
