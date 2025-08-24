namespace Analytics.Schemas
{
	public class SupportClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.supportClicked;
			}
		}
	}
}
