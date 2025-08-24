namespace Analytics.Schemas
{
	public class GuestPurchaseConfirmationOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.guestPurchaseConfirmationOpened;
			}
		}
	}
}
