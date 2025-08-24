using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PrivateMatchCreatedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.privateMatchCreated;
			}
		}

		public PrivateMatchCreatedSchema(MatchTypeParameter gameType, StageParameter stage, PasswordProtectedParameter passwordProtected, UserSkillParameter userSkill, UserLevelParameter userLevel)
			: base(gameType, stage, passwordProtected, userSkill, userLevel)
		{
		}
	}
}
