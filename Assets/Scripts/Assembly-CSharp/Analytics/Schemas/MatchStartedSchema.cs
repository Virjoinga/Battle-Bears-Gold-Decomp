using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MatchStartedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchStarted;
			}
		}

		public MatchStartedSchema(MatchIDParameter matchID, MatchNameParameter matchName, MatchTypeParameter matchType, IsPrivateMatchParameter isPrivateMatch, StageParameter stageParameter, LoadoutParameters loadoutParameters, ProModeStateParameter proModeState, UserSkillParameter userSkill, UserLevelParameter userLevel, UsersInMatchParameter usersInMatch)
		{
			_parameters.Add(matchID);
			_parameters.Add(matchName);
			_parameters.Add(matchType);
			_parameters.Add(isPrivateMatch);
			_parameters.Add(stageParameter);
			_parameters.AddRange(loadoutParameters);
			_parameters.Add(proModeState);
			_parameters.Add(userSkill);
			_parameters.Add(userLevel);
			_parameters.Add(usersInMatch);
		}
	}
}
