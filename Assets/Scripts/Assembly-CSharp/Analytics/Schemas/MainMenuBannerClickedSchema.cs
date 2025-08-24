using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MainMenuBannerClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.mainMenuBannerClicked;
			}
		}

		public MainMenuBannerClickedSchema(MainMenuBannerURLParameter mainMenuBannerURL)
			: base(mainMenuBannerURL)
		{
		}
	}
}
