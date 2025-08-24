namespace Analytics.Schemas
{
	public class GetJoulesOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.getJoulesOpened;
			}
		}
	}
}
