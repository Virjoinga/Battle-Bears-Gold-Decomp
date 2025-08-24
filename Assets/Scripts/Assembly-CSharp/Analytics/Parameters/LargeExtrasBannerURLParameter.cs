namespace Analytics.Parameters
{
	public class LargeExtrasBannerURLParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.largeExtrasBannerURL;
			}
		}

		public LargeExtrasBannerURLParameter(string value)
			: base(value)
		{
		}
	}
}
