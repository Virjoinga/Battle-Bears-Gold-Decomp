namespace Analytics.Schemas
{
	public class AccountCreationEmailsValidatedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.accountCreationEmailsValidated;
			}
		}
	}
}
