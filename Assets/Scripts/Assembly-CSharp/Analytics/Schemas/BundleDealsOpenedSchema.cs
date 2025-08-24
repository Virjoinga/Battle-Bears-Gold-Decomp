namespace Analytics.Schemas
{
	public class BundleDealsOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.bundleDealsOpened;
			}
		}
	}
}
