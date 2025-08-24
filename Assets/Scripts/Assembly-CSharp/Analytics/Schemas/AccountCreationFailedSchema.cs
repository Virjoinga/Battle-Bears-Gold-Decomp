using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class AccountCreationFailedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.accountCreationFailed;
			}
		}

		public AccountCreationFailedSchema(AccountCreationErrorParameter accountCreationError)
			: base(accountCreationError)
		{
		}
	}
}
