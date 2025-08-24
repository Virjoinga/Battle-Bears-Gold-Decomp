namespace Analytics.Parameters
{
	public class MatchExitReasonParameter : StringParameter
	{
		public enum Reason
		{
			USER_LEFT = 0,
			DISCONNECTED = 1
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchExitReason;
			}
		}

		public MatchExitReasonParameter(Reason value)
			: base(value.ToString())
		{
		}
	}
}
