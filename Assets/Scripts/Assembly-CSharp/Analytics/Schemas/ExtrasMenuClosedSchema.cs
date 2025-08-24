namespace Analytics.Schemas
{
	public class ExtrasMenuClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.extrasMenuClosed;
			}
		}
	}
}
