namespace Analytics.Schemas
{
	public class OutOfEnergyPopupClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.outOfEnergyPopupClosed;
			}
		}
	}
}
