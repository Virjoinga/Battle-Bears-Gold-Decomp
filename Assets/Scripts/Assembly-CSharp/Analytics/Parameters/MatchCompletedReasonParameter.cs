namespace Analytics.Parameters
{
	public class MatchCompletedReasonParameter : StringParameter
	{
		public enum Reason
		{
			TIMER_ENDED = 0,
			PLAYERS_LEFT = 1
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.matchCompletionReason;
			}
		}

		public MatchCompletedReasonParameter(Reason reason)
			: base(reason.ToString())
		{
		}
	}
}
