using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PrivateMatchSelectOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privateMatchSelectOpened;
			}
		}

		public PrivateMatchSelectOpenedSchema(UserSkillParameter userSkill, UserLevelParameter userLevel)
			: base(userSkill, userLevel)
		{
		}
	}
}
