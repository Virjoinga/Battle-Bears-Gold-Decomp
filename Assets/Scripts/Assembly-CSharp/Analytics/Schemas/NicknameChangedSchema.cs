using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class NicknameChangedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.nicknameChanged;
			}
		}

		public NicknameChangedSchema(UserDisplayNameParameter userDisplayName)
			: base(userDisplayName)
		{
		}
	}
}
