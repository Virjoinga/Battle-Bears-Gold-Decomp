namespace Analytics.Schemas
{
	public class AccountMenuOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.accountMenuOpened;
			}
		}
	}
}
