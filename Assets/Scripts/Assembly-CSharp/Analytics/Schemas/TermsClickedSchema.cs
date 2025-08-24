namespace Analytics.Schemas
{
	public class TermsClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.termsClicked;
			}
		}
	}
}
