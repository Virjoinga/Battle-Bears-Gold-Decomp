using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class AccountCreatedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.accountCreated;
			}
		}

		public AccountCreatedSchema(DeviceTotalTimePlayedParameter deviceTotalTimePlayed)
			: base(deviceTotalTimePlayed)
		{
		}
	}
}
