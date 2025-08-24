using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class ZoomModeChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.zoomModeChanged;
			}
		}

		public ZoomModeChangedSchema(ZoomModeParameter zoomMode)
			: base(zoomMode)
		{
		}
	}
}
