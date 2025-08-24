using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class CreatePrivateMatchOpenedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.createPrivateMatchOpened;
			}
		}

		public CreatePrivateMatchOpenedSchema(UserSkillParameter userSkill, UserLevelParameter userLevel)
			: base(userSkill, userLevel)
		{
		}
	}
}
