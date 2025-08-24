namespace Analytics.Schemas
{
	public class EnergyRefilledSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.energyRefilled;
			}
		}
	}
}
