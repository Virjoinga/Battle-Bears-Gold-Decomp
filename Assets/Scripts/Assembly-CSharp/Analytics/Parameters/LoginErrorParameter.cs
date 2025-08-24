namespace Analytics.Parameters
{
	public class LoginErrorParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.loginError;
			}
		}

		public LoginErrorParameter(string value)
			: base(value)
		{
		}
	}
}
