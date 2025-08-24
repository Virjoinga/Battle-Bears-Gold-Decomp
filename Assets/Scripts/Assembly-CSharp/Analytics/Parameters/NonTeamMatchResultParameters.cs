using System.Collections.Generic;
using Analytics.Parameters.Collections;
using UnityEngine;

namespace Analytics.Parameters
{
	public abstract class NonTeamMatchResultParameters : IEventParameterEnumerable
	{
		protected void AddMyParams(List<IEventParameter> paramsList, Report.Player me)
		{
			paramsList.Add(new MyDeathsParameter(me.TotalDeaths));
			paramsList.Add(new MyKillsParameter(me.TotalKills));
		}

		protected void AddWinningPlayerParams(List<IEventParameter> paramsList, Report.Player winningPlayer)
		{
			paramsList.Add(new WinningPlayerDeathsParameter(winningPlayer.TotalDeaths));
			paramsList.Add(new WinningPlayerKillsParameter(winningPlayer.TotalKills));
			paramsList.Add(new WinningPlayerLevelParameter(winningPlayer.level));
			paramsList.Add(new WinningPlayerSkillParameter(Mathf.FloorToInt(winningPlayer.skill)));
		}
	}
}
