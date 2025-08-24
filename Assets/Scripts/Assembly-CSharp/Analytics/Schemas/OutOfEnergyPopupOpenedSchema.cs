namespace Analytics.Schemas
{
	public class OutOfEnergyPopupOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.outOfEnergyPopupOpened;
			}
		}
	}
}
