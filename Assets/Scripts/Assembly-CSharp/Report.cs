using System.Collections.Generic;
using System.Xml.Serialization;
using JsonFx.Json;

public class Report
{
	public class Loadout
	{
		[XmlAttribute]
		public int skin = -1;

		[XmlAttribute]
		public int model = -1;

		[XmlAttribute]
		public int primary = -1;

		[XmlAttribute]
		public int secondary = -1;

		[XmlAttribute]
		public int melee = -1;

		[XmlAttribute]
		public int special = -1;

		[XmlAttribute]
		public int item1 = -1;

		[XmlAttribute]
		public int item2 = -1;
	}

	[XmlType("pid_map")]
	public class PidMap
	{
		[XmlAttribute]
		public int pid = -1;

		[XmlAttribute]
		public float damage;

		[XmlAttribute]
		public int kills;

		public PidMap(int pid, int kills, float dmg)
		{
			this.pid = pid;
			damage = dmg;
			this.kills = kills;
		}

		public PidMap()
		{
		}
	}

	[XmlType("player")]
	public class Player
	{
		[XmlAttribute]
		public int id = -1;

		[XmlAttribute]
		public int team_id = -1;

		public Loadout loadout = new Loadout();

		public List<PidMap> dealt = new List<PidMap>();

		[XmlAttribute]
		public int joulePacksCollected;

		[XmlAttribute]
		public int longestKillStreak;

		[XmlAttribute]
		public int killStreakBonusTotal;

		[XmlAttribute]
		public int stopStreakBonusTotal;

		[XmlAttribute]
		public int assistBonusTotal;

		[XmlAttribute]
		public int bombHoldBonusTotal;

		[XmlAttribute]
		public int bombDepositBonusTotal;

		[XmlAttribute]
		public string greeName = string.Empty;

		[XmlAttribute]
		public bool hasLeft;

		public int level = 1;

		public float skill;

		private PlayerStats stats;

		public int TotalKills
		{
			get
			{
				return stats.NetKills;
			}
		}

		public int TotalDeaths
		{
			get
			{
				return stats.NumDeaths;
			}
		}

		public float TotalDamageDealt
		{
			get
			{
				return stats.NetDamage;
			}
		}

		public Player()
		{
		}

		public Player(int key, PlayerStats playerStats, GameManager gm)
		{
			id = playerStats.playerLoadout.pid;
			team_id = (int)playerStats.playerTeam;
			stats = playerStats;
			loadout.model = playerStats.playerLoadout.model.id;
			loadout.skin = playerStats.playerLoadout.skin.id;
			loadout.primary = playerStats.playerLoadout.primary.id;
			loadout.secondary = playerStats.playerLoadout.secondary.id;
			loadout.melee = playerStats.playerLoadout.melee.id;
			foreach (KeyValuePair<int, float> item in playerStats.DamagesDealt)
			{
				int kills = 0;
				if (playerStats.KillsCaused.ContainsKey(item.Key))
				{
					kills = playerStats.KillsCaused[item.Key];
				}
				dealt.Add(new PidMap(gm.getPlayerIDFromPid(item.Key), kills, item.Value));
			}
			greeName = playerStats.greeName;
			level = playerStats.level;
			skill = playerStats.skill;
			joulePacksCollected = playerStats.joulePacksCollected;
			longestKillStreak = playerStats.longestKillStreak;
			killStreakBonusTotal = playerStats.killStreakBonusTotal;
			stopStreakBonusTotal = playerStats.stopStreakBonusTotal;
			assistBonusTotal = playerStats.assistBonusTotal;
			bombHoldBonusTotal = playerStats.bombHoldBonusTotal;
			bombDepositBonusTotal = playerStats.bombDepositBonusTotal;
			hasLeft = playerStats.hasLeft;
			if (playerStats.playerLoadout.special != null)
			{
				loadout.special = playerStats.playerLoadout.special.id;
			}
			if (playerStats.playerLoadout.equipment1 != null)
			{
				loadout.item1 = playerStats.playerLoadout.equipment1.id;
			}
			if (playerStats.playerLoadout.equipment2 != null)
			{
				loadout.item2 = playerStats.playerLoadout.equipment2.id;
			}
		}
	}

	[XmlType("team")]
	public class Team
	{
		[XmlAttribute]
		public int id = -1;

		[XmlAttribute]
		public int kills = -1;

		[XmlAttribute]
		public int bombsDeposited;

		[XmlAttribute]
		public int pointsCaptured;

		public Team()
		{
		}

		public Team(int id, int kills, int bombsDeposited, int pointsCaptured)
		{
			this.id = id;
			this.kills = kills;
			this.bombsDeposited = bombsDeposited;
			this.pointsCaptured = pointsCaptured;
		}
	}

	public class Summary
	{
		[XmlAttribute]
		public int win_team = -1;

		[XmlAttribute]
		public ulong match_start;

		[XmlAttribute]
		public ulong match_end;

		[XmlAttribute]
		public string gameMode = "FFA";

		public List<Team> teams = new List<Team>();
	}

	[XmlType("report")]
	public class GameReport
	{
		[XmlAttribute]
		public string game_name = string.Empty;

		[XmlAttribute]
		public string server_ip = string.Empty;

		[XmlAttribute]
		public string match_server = string.Empty;

		public List<Player> players = new List<Player>();

		public Summary summary = new Summary();

		public string MapName
		{
			get
			{
				string[] array = game_name.Split(':');
				if (array.Length > 0)
				{
					return array[0];
				}
				return string.Empty;
			}
		}

		public virtual string GetJSON()
		{
			return JsonWriter.Serialize(this);
		}
	}

	public class NonTeamGameReport : GameReport
	{
		public int winner_id;

		public NonTeamGameReport()
		{
		}

		public NonTeamGameReport(int winnerId)
		{
			winner_id = winnerId;
		}

		public override string GetJSON()
		{
			return JsonWriter.Serialize(this);
		}
	}

	public class RoyaleGameReport : NonTeamGameReport
	{
		public List<DeathTimesMap> deathTimes;

		public RoyaleGameReport()
		{
		}

		public RoyaleGameReport(int winnerId, List<DeathTimesMap> dt)
			: base(winnerId)
		{
			deathTimes = dt;
		}

		public override string GetJSON()
		{
			return JsonWriter.Serialize(this);
		}
	}

	public class DeathTimesMap
	{
		public int deathTime;

		public int id;
	}

	public static GameReport GenerateGameReport(GameManager gm)
	{
		GameReport gameReport = ((Preferences.Instance.CurrentGameMode == GameMode.ROYL) ? new RoyaleGameReport(gm.GetRoyaleWinner().id, gm.DeathTimesForReport()) : ((Preferences.Instance.CurrentGameMode != 0) ? new GameReport() : new NonTeamGameReport(gm.GetFFALeader().id)));
		gameReport.server_ip = ServiceManager.Instance.GetMatchGameServer();
		gameReport.game_name = ServiceManager.Instance.GetMatchGameName();
		gameReport.match_server = ServiceManager.Instance.GetLastMatchServerUsed();
		foreach (KeyValuePair<int, PlayerStats> playerStat in gm.playerStats)
		{
			gameReport.players.Add(new Player(playerStat.Key, playerStat.Value, gm));
		}
		foreach (Team team in gm.Teams)
		{
			gameReport.summary.teams.Add(team);
		}
		gameReport.summary.win_team = gm.WinningTeam.id;
		gameReport.summary.match_start = (ulong)gm.StartTime;
		gameReport.summary.match_end = (ulong)gm.EndTime;
		gameReport.summary.gameMode = Preferences.Instance.CurrentGameModeStr;
		return gameReport;
	}
}
