namespace Analytics.Parameters
{
	public class ZoomModeParameter : BooleanParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.zoomMode;
			}
		}

		public ZoomModeParameter(bool zoomIsOn)
			: base(zoomIsOn)
		{
		}
	}
}
