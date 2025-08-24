namespace Analytics.Parameters
{
	public class SessionTimePlayedParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.sessionTimePlayed;
			}
		}

		public SessionTimePlayedParameter(int timePlayed)
			: base(timePlayed)
		{
		}
	}
}
