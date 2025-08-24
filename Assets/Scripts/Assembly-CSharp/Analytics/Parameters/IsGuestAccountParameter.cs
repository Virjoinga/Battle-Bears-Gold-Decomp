namespace Analytics.Parameters
{
	public class IsGuestAccountParameter : BooleanParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.isGuestAccount;
			}
		}

		public IsGuestAccountParameter(bool value)
			: base(value)
		{
		}
	}
}
