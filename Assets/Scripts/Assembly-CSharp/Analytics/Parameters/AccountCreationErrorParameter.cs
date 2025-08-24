namespace Analytics.Parameters
{
	public class AccountCreationErrorParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.accountCreationError;
			}
		}

		public AccountCreationErrorParameter(string value)
			: base(value)
		{
		}
	}
}
