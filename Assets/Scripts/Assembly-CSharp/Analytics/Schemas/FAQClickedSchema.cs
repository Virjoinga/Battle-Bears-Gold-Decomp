namespace Analytics.Schemas
{
	public class FAQClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.faqClicked;
			}
		}
	}
}
