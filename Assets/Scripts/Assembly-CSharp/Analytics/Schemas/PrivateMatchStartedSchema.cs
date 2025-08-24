using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PrivateMatchStartedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privateMatchStarted;
			}
		}

		public PrivateMatchStartedSchema(PrivateMatchWaitTimeParameter privateMatchWaitTime, UsersInMatchParameter usersInMatch)
			: base(privateMatchWaitTime, usersInMatch)
		{
		}
	}
}
