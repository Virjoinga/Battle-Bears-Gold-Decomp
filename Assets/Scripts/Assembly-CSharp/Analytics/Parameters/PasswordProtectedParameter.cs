namespace Analytics.Parameters
{
	public class PasswordProtectedParameter : BooleanParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.passwordProtected;
			}
		}

		public PasswordProtectedParameter(bool value)
			: base(value)
		{
		}
	}
}
