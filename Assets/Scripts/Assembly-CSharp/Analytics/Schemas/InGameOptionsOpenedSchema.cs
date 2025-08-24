namespace Analytics.Schemas
{
	public class InGameOptionsOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.inGameOptionsOpened;
			}
		}
	}
}
