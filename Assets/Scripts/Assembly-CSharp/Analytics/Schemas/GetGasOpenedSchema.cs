namespace Analytics.Schemas
{
	public class GetGasOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.getGasOpened;
			}
		}
	}
}
