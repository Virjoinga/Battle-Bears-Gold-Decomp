using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class SmallExtrasBannerClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.smallExtrasBannerClicked;
			}
		}

		public SmallExtrasBannerClickedSchema(SmallExtrasBannerURLParameter smallExtrasBannerURL)
			: base(smallExtrasBannerURL)
		{
		}
	}
}
