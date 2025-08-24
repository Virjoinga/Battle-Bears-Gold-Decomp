using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PrivateMatchLeftSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privateMatchLeft;
			}
		}

		public PrivateMatchLeftSchema(PrivateMatchWaitTimeParameter privateMatchWaitTime)
			: base(privateMatchWaitTime)
		{
		}
	}
}
