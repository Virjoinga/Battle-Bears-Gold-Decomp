namespace Analytics.Schemas
{
	public class GetGasClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.getGasClosed;
			}
		}
	}
}
