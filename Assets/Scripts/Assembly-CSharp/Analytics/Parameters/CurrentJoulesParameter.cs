namespace Analytics.Parameters
{
	public class CurrentJoulesParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.currentJoules;
			}
		}

		public CurrentJoulesParameter(int amount)
			: base(amount)
		{
		}
	}
}
