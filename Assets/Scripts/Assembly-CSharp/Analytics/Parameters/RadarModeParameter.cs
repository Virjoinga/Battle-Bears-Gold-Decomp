namespace Analytics.Parameters
{
	public class RadarModeParameter : BooleanParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.radarMode;
			}
		}

		public RadarModeParameter(bool radarIsOn)
			: base(radarIsOn)
		{
		}
	}
}
