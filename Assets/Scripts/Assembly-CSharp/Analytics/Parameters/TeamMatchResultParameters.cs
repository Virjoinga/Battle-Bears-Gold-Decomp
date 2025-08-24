using System.Collections.Generic;
using System.Linq;
using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class TeamMatchResultParameters : IEventParameterEnumerable
	{
		public TeamMatchResultParameters(Report.GameReport report, GameMode gameMode, int myId)
		{
			List<IEventParameter> list = new List<IEventParameter>();
			Report.Player player = report.players.First((Report.Player p) => p.id == myId);
			int team_id = player.team_id;
			AddMyParams(list, player);
			list.AddRange(new TeamsParameters(report.players, team_id));
			foreach (Report.Team team in report.summary.teams)
			{
				AddTeamParams(list, gameMode, team, team_id);
			}
			_eventParameters = list.ToArray();
		}

		private void AddMyParams(List<IEventParameter> paramsList, Report.Player me)
		{
			paramsList.Add(new MyDeathsParameter(me.TotalDeaths));
			paramsList.Add(new MyKillsParameter(me.TotalKills));
		}

		private void AddTeamParams(List<IEventParameter> paramsList, GameMode gameMode, Report.Team team, int myTeam)
		{
			if (team.id == myTeam)
			{
				paramsList.Add(new MyTeamKillsParameter(team.kills));
				switch (gameMode)
				{
				case GameMode.CTF:
					paramsList.Add(new MyTeamBombsDepositedParameter(team.bombsDeposited));
					break;
				case GameMode.KOTH:
					paramsList.Add(new MyTeamPointsCapturedParameter(team.pointsCaptured));
					break;
				}
			}
			else
			{
				paramsList.Add(new OpposingTeamKillsParameter(team.kills));
				switch (gameMode)
				{
				case GameMode.CTF:
					paramsList.Add(new OpposingTeamBombsDepositedParameter(team.bombsDeposited));
					break;
				case GameMode.KOTH:
					paramsList.Add(new OpposingTeamPointsCapturedParameter(team.pointsCaptured));
					break;
				}
			}
		}
	}
}
