namespace Analytics.Schemas
{
	public class MatchSelectOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchSelectOpened;
			}
		}
	}
}
