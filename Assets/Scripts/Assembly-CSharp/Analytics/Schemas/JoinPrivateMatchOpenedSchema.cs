using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class JoinPrivateMatchOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.joinPrivateMatchOpened;
			}
		}

		public JoinPrivateMatchOpenedSchema(UserSkillParameter userSkill, UserLevelParameter userLevel)
			: base(userSkill, userLevel)
		{
		}
	}
}
