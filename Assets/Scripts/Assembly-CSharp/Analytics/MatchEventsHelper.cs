using System;
using System.Collections.Generic;
using System.Linq;
using Analytics.Parameters;
using Analytics.Schemas;
using Utils.Comparers;

namespace Analytics
{
	public class MatchEventsHelper : EventHelper
	{
		public class MatchInfo
		{
			public readonly string MatchName;

			public readonly string MatchId;

			public readonly string Stage;

			public readonly GameMode Mode;

			public readonly Stats Stats;

			public readonly int MyId;

			public readonly bool IsPrivate;

			public MatchInfo()
			{
				string matchGameName = ServiceManager.Instance.GetMatchGameName();
				string[] array = matchGameName.Split(':');
				Stage = array[0];
				IsPrivate = ServiceManager.Instance.IsPrivateMatch;
				string text = ((!IsPrivate) ? array[array.Length - 1] : matchGameName.Split('|')[1]);
				MatchName = Stage + ":" + text;
				MatchId = EventHelper.Hash(MatchName);
				Mode = Preferences.Instance.CurrentGameMode;
				Stats = EventHelper.GetStats();
				MyId = GameManager.Instance.localPlayerID;
			}
		}

		public static MatchStartedSchema MatchStarted()
		{
			MatchInfo matchInfo = new MatchInfo();
			return new MatchStartedSchema(new MatchIDParameter(matchInfo.MatchId), new MatchNameParameter(matchInfo.MatchName), new MatchTypeParameter(matchInfo.Mode), new IsPrivateMatchParameter(matchInfo.IsPrivate), new StageParameter(matchInfo.Stage), new LoadoutParameters(LoadoutManager.Instance.CurrentLoadout), ProModeState(), new UserSkillParameter(matchInfo.Stats.skill), new UserLevelParameter(matchInfo.Stats.level), new UsersInMatchParameter(PhotonNetwork.playerList.Length));
		}

		public static MatchExitedSchema MatchExited(bool wasDisconnect)
		{
			MatchInfo matchInfo = new MatchInfo();
			List<PlayerCharacterManager> playerCharacterManagers = GameManager.Instance.GetPlayerCharacterManagers();
			return new MatchExitedSchema(new MatchIDParameter(matchInfo.MatchId), new MatchNameParameter(matchInfo.MatchName), new MatchTypeParameter(matchInfo.Mode), new MatchExitReasonParameter(wasDisconnect ? MatchExitReasonParameter.Reason.DISCONNECTED : MatchExitReasonParameter.Reason.USER_LEFT), new IsPrivateMatchParameter(matchInfo.IsPrivate), new StageParameter(matchInfo.Stage), new LoadoutParameters(LoadoutManager.Instance.CurrentLoadout), ProModeState(), new UserSkillParameter(matchInfo.Stats.skill), new UserLevelParameter(matchInfo.Stats.level), new UsersInMatchParameter(PhotonNetwork.playerList.Length), (!matchInfo.Mode.IsTeam()) ? null : new TeamsParameters(playerCharacterManagers, matchInfo.MyId), matchInfo.Mode.IsTeam() ? null : new OppositionParameters(playerCharacterManagers, matchInfo.MyId));
		}

		public static MatchCompletedSchema MatchCompleted(Report.GameReport report)
		{
			MatchInfo matchInfo = new MatchInfo();
			NonTeamMatchResultParameters nonTeamMatchResultParameters = null;
			if (!matchInfo.Mode.IsTeam())
			{
				if (matchInfo.Mode == GameMode.FFA)
				{
					nonTeamMatchResultParameters = new FFAMatchResultParameters(report, matchInfo.MyId);
				}
				else
				{
					if (matchInfo.Mode != GameMode.ROYL)
					{
						throw new Exception("No match result defined for game mode " + matchInfo.Mode);
					}
					nonTeamMatchResultParameters = new RoyaleMatchResultParameters(report, matchInfo.MyId);
				}
			}
			return new MatchCompletedSchema(new MatchIDParameter(matchInfo.MatchId), new MatchNameParameter(matchInfo.MatchName), new MatchTypeParameter(matchInfo.Mode), MatchCompletedReason(report, matchInfo.MyId), MatchResult(report, matchInfo), new IsPrivateMatchParameter(matchInfo.IsPrivate), new StageParameter(matchInfo.Stage), new LoadoutParameters(LoadoutManager.Instance.CurrentLoadout), ProModeState(), new UserSkillParameter(matchInfo.Stats.skill), new UserLevelParameter(matchInfo.Stats.level), new UsersInMatchParameter(PhotonNetwork.playerList.Length), matchInfo.Mode.IsTeam() ? null : nonTeamMatchResultParameters, (!matchInfo.Mode.IsTeam()) ? null : new TeamMatchResultParameters(report, matchInfo.Mode, matchInfo.MyId));
		}

		private static ProModeStateParameter ProModeState()
		{
			List<Item> proMode = Store.Instance.proMode;
			bool ownsJump = false;
			bool ownsRadar = false;
			foreach (Item item in proMode)
			{
				switch (item.name)
				{
				case "jump":
					ownsJump = ServiceManager.Instance.IsItemBought(item.id);
					break;
				case "radar":
					ownsRadar = ServiceManager.Instance.IsItemBought(item.id);
					break;
				}
			}
			return new ProModeStateParameter(ownsJump, ownsRadar);
		}

		private static MatchResultParameter MatchResult(Report.GameReport report, MatchInfo info)
		{
			switch (info.Mode)
			{
			case GameMode.FFA:
				return FFAMatchResult(report, info.MyId);
			case GameMode.TB:
			case GameMode.CTF:
			case GameMode.KOTH:
				return TeamMatchResult(report, info.MyId);
			case GameMode.ROYL:
				return RoyaleMatchResult(report, info.MyId);
			default:
				throw new Exception("No MatchResultParameter function defined for game mode " + info.Mode);
			}
		}

		private static MatchResultParameter FFAMatchResult(Report.GameReport report, int myId)
		{
			List<Report.Player> list = new List<Report.Player>(report.players);
			Report.Player a = list.First((Report.Player p) => p.id == myId);
			list.Sort(new FFAPlayerReportComparer());
			FFATruePlayerComparer fFATruePlayerComparer = new FFATruePlayerComparer();
			switch (fFATruePlayerComparer.Compare(a, list[0]))
			{
			case -1:
				return new MatchResultParameter(MatchResultParameter.Result.WIN);
			case 0:
				return new MatchResultParameter(MatchResultParameter.Result.DRAW);
			default:
				return new MatchResultParameter(MatchResultParameter.Result.LOSE);
			}
		}

		private static MatchResultParameter TeamMatchResult(Report.GameReport report, int myId)
		{
			List<Report.Player> source = new List<Report.Player>(report.players);
			Report.Player player = source.First((Report.Player p) => p.id == myId);
			return new MatchResultParameter((player.team_id != GameManager.Instance.WinningTeam.id) ? MatchResultParameter.Result.LOSE : MatchResultParameter.Result.WIN);
		}

		private static MatchResultParameter RoyaleMatchResult(Report.GameReport report, int myId)
		{
			List<Report.Player> list = new List<Report.Player>(report.players);
			Report.Player player = list.First((Report.Player p) => p.id == myId);
			list.Sort(new RoyalePlayerReportComparer(report as Report.RoyaleGameReport));
			return new MatchResultParameter((list[0].id != myId) ? MatchResultParameter.Result.LOSE : MatchResultParameter.Result.WIN);
		}

		private static MatchCompletedReasonParameter MatchCompletedReason(Report.GameReport report, int myId)
		{
			bool flag = true;
			int team_id = report.players.First((Report.Player p) => p.id == myId).team_id;
			foreach (Report.Player player in report.players)
			{
				if (player.team_id != team_id && !player.hasLeft)
				{
					flag = false;
					break;
				}
			}
			return new MatchCompletedReasonParameter(flag ? MatchCompletedReasonParameter.Reason.PLAYERS_LEFT : MatchCompletedReasonParameter.Reason.TIMER_ENDED);
		}
	}
}
