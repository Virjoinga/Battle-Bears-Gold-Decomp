namespace Analytics.Schemas
{
	public class ExtrasMenuOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.extrasMenuOpened;
			}
		}
	}
}
