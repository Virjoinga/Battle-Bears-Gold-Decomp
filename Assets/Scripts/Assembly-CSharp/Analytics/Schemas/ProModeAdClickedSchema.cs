namespace Analytics.Schemas
{
	public class ProModeAdClickedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.proModeAdClicked;
			}
		}
	}
}
