namespace Analytics.Parameters
{
	public class SmallExtrasBannerURLParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.smallExtrasBannerURL;
			}
		}

		public SmallExtrasBannerURLParameter(string value)
			: base(value)
		{
		}
	}
}
