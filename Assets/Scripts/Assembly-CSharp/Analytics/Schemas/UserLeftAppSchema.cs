using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class UserLeftAppSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.userLeftApp;
			}
		}

		public UserLeftAppSchema(SessionTimePlayedParameter sessionTimePlayed, LastEventSentParameter lastEventSent)
			: base(sessionTimePlayed, lastEventSent)
		{
		}
	}
}
