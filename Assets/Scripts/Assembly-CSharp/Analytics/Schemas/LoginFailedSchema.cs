using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class LoginFailedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.loginFailed;
			}
		}

		public LoginFailedSchema(LoginErrorParameter loginError)
			: base(loginError)
		{
		}
	}
}
