using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MatchCompletedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchCompleted;
			}
		}

		public MatchCompletedSchema(MatchIDParameter matchID, MatchNameParameter matchName, MatchTypeParameter matchType, MatchCompletedReasonParameter matchCompletedReason, MatchResultParameter matchResult, IsPrivateMatchParameter isPrivateMatch, StageParameter stageParameter, LoadoutParameters loadoutParameters, ProModeStateParameter proModeState, UserSkillParameter userSkill, UserLevelParameter userLevel, UsersInMatchParameter usersInMatch, NonTeamMatchResultParameters nonTeamParams = null, TeamMatchResultParameters teamParams = null)
		{
			_parameters.Add(matchID);
			_parameters.Add(matchName);
			_parameters.Add(matchType);
			_parameters.Add(matchCompletedReason);
			_parameters.Add(matchResult);
			_parameters.Add(isPrivateMatch);
			_parameters.Add(stageParameter);
			_parameters.AddRange(loadoutParameters);
			_parameters.Add(proModeState);
			_parameters.Add(userSkill);
			_parameters.Add(userLevel);
			_parameters.Add(usersInMatch);
			if (nonTeamParams != null)
			{
				_parameters.AddRange(nonTeamParams);
			}
			if (teamParams != null)
			{
				_parameters.AddRange(teamParams);
			}
		}
	}
}
