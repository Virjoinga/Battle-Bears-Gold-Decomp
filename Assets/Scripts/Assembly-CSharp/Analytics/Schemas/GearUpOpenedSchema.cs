namespace Analytics.Schemas
{
	public class GearUpOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.gearUpOpened;
			}
		}
	}
}
