using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class BundleDetailsOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.bundleDetailsOpened;
			}
		}

		public BundleDetailsOpenedSchema(ItemNameParameter itemName)
			: base(itemName)
		{
		}
	}
}
