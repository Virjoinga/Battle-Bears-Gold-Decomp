using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class PlayerDiedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.playerDied;
			}
		}

		public PlayerDiedSchema(IsSuicideParameter isSuicide, UserLevelParameter userLevel, UserSkillParameter userSkill, KillerLoadoutParameters killerLoadout, KillerLevelParameter killerLevel, KillerSkillParameter killerSkill)
		{
			_parameters.Add(isSuicide);
			_parameters.Add(userLevel);
			_parameters.Add(userSkill);
			if (killerLoadout != null)
			{
				_parameters.AddRange(killerLoadout);
			}
			if (killerLevel != null)
			{
				_parameters.Add(killerLevel);
			}
			if (killerSkill != null)
			{
				_parameters.Add(killerSkill);
			}
		}
	}
}
