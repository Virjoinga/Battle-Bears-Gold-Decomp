namespace Analytics.Parameters
{
	public class MainMenuBannerURLParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.mainMenuBannerURL;
			}
		}

		public MainMenuBannerURLParameter(string value)
			: base(value)
		{
		}
	}
}
