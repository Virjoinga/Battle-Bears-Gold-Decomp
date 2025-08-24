namespace Analytics.Schemas
{
	public class GuestPurchaseConfirmationClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.guestPurchaseConfirmationClosed;
			}
		}
	}
}
