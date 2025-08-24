namespace Analytics.Parameters
{
	public class LastEventSentParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.lastEventSent;
			}
		}

		public LastEventSentParameter(AnalyticsEvent evt)
			: base(evt.ToString())
		{
		}
	}
}
