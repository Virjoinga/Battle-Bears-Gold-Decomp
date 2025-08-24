namespace Analytics.Schemas
{
	public class GetJoulesClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.getJoulesClosed;
			}
		}
	}
}
