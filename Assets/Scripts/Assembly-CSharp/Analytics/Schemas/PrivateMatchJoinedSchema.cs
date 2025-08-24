using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PrivateMatchJoinedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privateMatchJoined;
			}
		}

		public PrivateMatchJoinedSchema(UserSkillParameter userSkill, UserLevelParameter userLevel)
			: base(userSkill, userLevel)
		{
		}
	}
}
