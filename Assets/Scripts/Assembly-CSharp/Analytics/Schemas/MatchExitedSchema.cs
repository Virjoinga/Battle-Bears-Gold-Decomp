using Analytics.Parameters;

namespace Analytics.Schemas
{
	public class MatchExitedSchema : EventSchema
	{
		public override AnalyticsEvent Name
		{
			get
			{
				return AnalyticsEvent.matchExited;
			}
		}

		public MatchExitedSchema(MatchIDParameter matchID, MatchNameParameter matchName, MatchTypeParameter matchType, MatchExitReasonParameter matchExitReason, IsPrivateMatchParameter isPrivateMatch, StageParameter stageParameter, LoadoutParameters loadoutParameters, ProModeStateParameter proModeState, UserSkillParameter userSkill, UserLevelParameter userLevel, UsersInMatchParameter usersInMatch, TeamsParameters teamsParameters = null, OppositionParameters oppositionParameters = null)
		{
			_parameters.Add(matchID);
			_parameters.Add(matchName);
			_parameters.Add(matchType);
			_parameters.Add(matchExitReason);
			_parameters.Add(isPrivateMatch);
			_parameters.Add(stageParameter);
			_parameters.AddRange(loadoutParameters);
			_parameters.Add(proModeState);
			_parameters.Add(userSkill);
			_parameters.Add(userLevel);
			_parameters.Add(usersInMatch);
			if (teamsParameters != null)
			{
				_parameters.AddRange(teamsParameters);
			}
			if (oppositionParameters != null)
			{
				_parameters.AddRange(oppositionParameters);
			}
		}
	}
}
