namespace Analytics.Schemas
{
	public class AccountMenuClosedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.accountMenuClosed;
			}
		}
	}
}
