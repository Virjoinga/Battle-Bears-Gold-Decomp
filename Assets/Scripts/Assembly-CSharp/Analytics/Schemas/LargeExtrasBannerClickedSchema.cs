using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class LargeExtrasBannerClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.largeExtrasBannerClicked;
			}
		}

		public LargeExtrasBannerClickedSchema(LargeExtrasBannerURLParameter largeExtrasBannerURL)
			: base(largeExtrasBannerURL)
		{
		}
	}
}
