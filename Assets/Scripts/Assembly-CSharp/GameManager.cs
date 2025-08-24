using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using UnityEngine;
using Utils.Comparers;

public class GameManager : MonoBehaviour
{
	public const float SECONDS_BEFORE_REPORT_SUBMIT = 2.5f;

	private const int ROYALE_RADAR_THRESHOLD = 4;

	private static GameManager instance;

	public int localPlayerID = -10;

	public AudioClip music;

	private Dictionary<Team, Report.Team> teams = new Dictionary<Team, Report.Team>();

	private Dictionary<int, int> pidToPlayerIDMap = new Dictionary<int, int>();

	private Dictionary<int, PlayerCharacterManager> players = new Dictionary<int, PlayerCharacterManager>();

	private Dictionary<int, int> _deathTimes = new Dictionary<int, int>();

	public float friendlyFireRatio;

	private bool isSynchronized;

	public Dictionary<int, PlayerStats> playerStats = new Dictionary<int, PlayerStats>();

	private bool gameSubmitted;

	private int gameStartTime;

	private int currentServerTime;

	private int gameLength_ms = 3000000;

	public bool IsSynchronized
	{
		get
		{
			return isSynchronized;
		}
		set
		{
			isSynchronized = value;
		}
	}

	private GameMode _gameMode
	{
		get
		{
			return Preferences.Instance.CurrentGameMode;
		}
	}

	public bool LocalPlayerWon
	{
		get
		{
			Report.Team winningTeam = WinningTeam;
			if (winningTeam == null)
			{
				Debug.LogError("[GameManager].LocalPlayerWon error: WinningTeam is null");
				return false;
			}
			PlayerCharacterManager playerCharacterManager = LocalPlayerCharacterManager();
			if (playerCharacterManager == null)
			{
				Debug.LogError("[GameManager].LocalPlayerWon error: LocalPlayerCharacterManager() is null");
				return false;
			}
			if (playerCharacterManager.PlayerController == null)
			{
				Debug.LogError("[GameManager].LocalPlayerWon error: LocalPlayerCharacterManager().PlayerController is null");
				return false;
			}
			return winningTeam.id == (int)playerCharacterManager.PlayerController.Team;
		}
	}

	public int CurrentServerTime
	{
		get
		{
			return currentServerTime;
		}
	}

	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}

	public bool IsGameSubmitted
	{
		get
		{
			return gameSubmitted;
		}
	}

	public int StartTime
	{
		get
		{
			return gameStartTime;
		}
		set
		{
			gameStartTime = value;
		}
	}

	public int TimeLeft
	{
		get
		{
			return (EndTime - currentServerTime) / 1000;
		}
	}

	public int EndTime
	{
		get
		{
			return gameStartTime + gameLength_ms;
		}
	}

	public int BlueDeposits
	{
		get
		{
			return teams[Team.BLUE].bombsDeposited;
		}
		set
		{
			teams[Team.BLUE].bombsDeposited = value;
			HUD.Instance.OnSetScore(Team.BLUE, value);
		}
	}

	public int RedDeposits
	{
		get
		{
			return teams[Team.RED].bombsDeposited;
		}
		set
		{
			teams[Team.RED].bombsDeposited = value;
			HUD.Instance.OnSetScore(Team.RED, value);
		}
	}

	public int BlueKills
	{
		get
		{
			return teams[Team.BLUE].kills;
		}
		set
		{
			teams[Team.BLUE].kills = value;
			HUD.Instance.OnSetScore(Team.BLUE, value);
		}
	}

	public int RedKills
	{
		get
		{
			return teams[Team.RED].kills;
		}
		set
		{
			teams[Team.RED].kills = value;
			HUD.Instance.OnSetScore(Team.RED, value);
		}
	}

	public List<Report.Team> Teams
	{
		get
		{
			return new List<Report.Team>(teams.Values);
		}
	}

	public Report.Team WinningTeam
	{
		get
		{
			if (_gameMode == GameMode.KOTH)
			{
				if (teams[Team.RED].pointsCaptured > teams[Team.BLUE].pointsCaptured)
				{
					return teams[Team.RED];
				}
				if (teams[Team.RED].pointsCaptured < teams[Team.BLUE].pointsCaptured)
				{
					return teams[Team.BLUE];
				}
			}
			else if (_gameMode == GameMode.CTF)
			{
				if (teams[Team.RED].bombsDeposited > teams[Team.BLUE].bombsDeposited)
				{
					return teams[Team.RED];
				}
				if (teams[Team.RED].bombsDeposited < teams[Team.BLUE].bombsDeposited)
				{
					return teams[Team.BLUE];
				}
			}
			if (_gameMode.IsTeam())
			{
				if (teams[Team.RED].kills > teams[Team.BLUE].kills)
				{
					return teams[Team.RED];
				}
				if (teams[Team.RED].kills < teams[Team.BLUE].kills)
				{
					return teams[Team.BLUE];
				}
				float num = 0f;
				float num2 = 0f;
				foreach (KeyValuePair<int, PlayerStats> playerStat in Instance.playerStats)
				{
					if (playerStat.Value.playerTeam == Team.RED)
					{
						num += playerStat.Value.NetDamage;
					}
					else if (playerStat.Value.playerTeam == Team.BLUE)
					{
						num2 += playerStat.Value.NetDamage;
					}
				}
				if (num > num2)
				{
					return teams[Team.RED];
				}
				return teams[Team.BLUE];
			}
			PlayerStats playerStats = null;
			if (_gameMode == GameMode.FFA)
			{
				playerStats = GetFFALeader();
			}
			else if (_gameMode == GameMode.ROYL)
			{
				playerStats = GetRoyaleWinner();
			}
			if (playerStats != null)
			{
				return teams[playerStats.playerTeam];
			}
			return teams[Team.BLUE];
		}
	}

	public List<Report.DeathTimesMap> DeathTimesForReport()
	{
		List<Report.DeathTimesMap> list = new List<Report.DeathTimesMap>();
		foreach (KeyValuePair<int, PlayerStats> playerStat in playerStats)
		{
			Report.DeathTimesMap deathTimesMap = new Report.DeathTimesMap();
			deathTimesMap.id = playerStat.Key;
			deathTimesMap.deathTime = ((!_deathTimes.ContainsKey(deathTimesMap.id)) ? int.MaxValue : _deathTimes[deathTimesMap.id]);
			list.Add(deathTimesMap);
		}
		return list;
	}

	public int RoyalePlacement(int id)
	{
		if (_deathTimes.ContainsKey(id))
		{
			int num = _deathTimes[id];
			int num2 = 0;
			foreach (KeyValuePair<int, int> deathTime in _deathTimes)
			{
				if (deathTime.Key != id && deathTime.Value < num)
				{
					num2++;
				}
			}
			return playerStats.Count - num2;
		}
		return 2;
	}

	public int RoyaleRemainingPlayers()
	{
		return playerStats.Count - _deathTimes.Count;
	}

	public bool WillSendReport()
	{
		return RoyaleRemainingPlayers() <= 2;
	}

	public int getPlayerIDFromPid(int pid)
	{
		if (pidToPlayerIDMap.ContainsKey(pid))
		{
			return pidToPlayerIDMap[pid];
		}
		Debug.LogWarning("couldn't find a mapping from pid: " + pid + " this should not happen");
		return -1;
	}

	public void AddPlayer(int id, PlayerCharacterManager pMgr)
	{
		players.Add(id, pMgr);
		pidToPlayerIDMap.Add(id, pMgr.playerLoadout.pid);
	}

	public List<int> GetPlayerIDs()
	{
		return new List<int>(players.Keys);
	}

	public PlayerCharacterManager Players(int id)
	{
		if (players.ContainsKey(id))
		{
			return players[id];
		}
		return null;
	}

	public PlayerCharacterManager LocalPlayerCharacterManager()
	{
		return Players(localPlayerID);
	}

	public List<PlayerCharacterManager> GetPlayerCharacterManagers()
	{
		return new List<PlayerCharacterManager>(players.Values);
	}

	public void SetGameLength(float minutes, float seconds)
	{
		gameLength_ms = (int)((60f * minutes + seconds) * 1000f);
		StartCoroutine(backupEndGameTimeout(60f * minutes + seconds + 60f));
	}

	private IEnumerator backupEndGameTimeout(float delay)
	{
		yield return new WaitForSeconds(delay);
		EndGame();
	}

	protected void EndGame()
	{
		StartCoroutine(SubmitReport());
	}

	public PlayerStats GetRoyaleWinner()
	{
		List<PlayerStats> list = new List<PlayerStats>(playerStats.Values);
		list.Sort(new RoyalePlayerStatsComparer(_deathTimes));
		return list[0];
	}

	public PlayerStats GetFFALeader()
	{
		List<PlayerStats> list = new List<PlayerStats>(playerStats.Values);
		list.Sort(new FFAPlayerStatsComparer());
		return list[0];
	}

	private void Awake()
	{
		instance = this;
		teams.Clear();
		teams.Add(Team.RED, new Report.Team(0, 0, 0, 0));
		teams.Add(Team.BLUE, new Report.Team(1, 0, 0, 0));
		ServiceManager.Instance.UpdateProperty("friendly_fire_ratio", ref friendlyFireRatio);
		SetupTimedGame();
	}

	private void SetupTimedGame()
	{
		int val = 0;
		int val2 = 0;
		switch (_gameMode)
		{
		case GameMode.FFA:
			ServiceManager.Instance.UpdateProperty("ffa_mode_length_seconds", ref val);
			ServiceManager.Instance.UpdateProperty("ffa_mode_length_minutes", ref val2);
			break;
		case GameMode.TB:
			ServiceManager.Instance.UpdateProperty("game_length_seconds", ref val);
			ServiceManager.Instance.UpdateProperty("game_length_minutes", ref val2);
			break;
		case GameMode.CTF:
			ServiceManager.Instance.UpdateProperty("bomb_mode_game_seconds", ref val);
			ServiceManager.Instance.UpdateProperty("bomb_mode_game_minutes", ref val2);
			break;
		case GameMode.KOTH:
			ServiceManager.Instance.UpdateProperty("koth_mode_length_seconds", ref val);
			ServiceManager.Instance.UpdateProperty("koth_mode_length_minutes", ref val2);
			break;
		case GameMode.ROYL:
			val = 0;
			ServiceManager.Instance.UpdateProperty("royale_mode_length_minutes", ref val2);
			break;
		default:
			throw new Exception("Time not set up for mode " + _gameMode);
		}
		if (val2 == 0 && val == 0)
		{
			if (_gameMode == GameMode.CTF)
			{
				val2 = 5;
			}
			else if (_gameMode == GameMode.TB)
			{
				val2 = 4;
			}
			else if (_gameMode == GameMode.FFA)
			{
				val2 = 4;
			}
		}
		SetGameLength(val2, val);
	}

	private void Start()
	{
		StartCoroutine(delayedHUDLoad());
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			StartCoroutine(PeriodicAloneCheck());
		}
	}

	private IEnumerator PeriodicAloneCheck()
	{
		while (!isSynchronized)
		{
			yield return new WaitForSeconds(2f);
		}
		while (!gameSubmitted)
		{
			yield return new WaitForSeconds(2f);
			checkIfAllPlayersLeft();
		}
	}

	private IEnumerator delayedHUDLoad()
	{
		yield return new WaitForSeconds(0.1f);
		GameObject obj = UnityEngine.Object.Instantiate(Resources.Load("HUDSystem"), Vector3.zero, Quaternion.identity) as GameObject;
		foreach (Transform child in obj.transform)
		{
			child.parent = null;
		}
		HUD.Instance.OnSetScore(Team.RED, 0);
		HUD.Instance.OnSetScore(Team.BLUE, 0);
		if (base.GetComponent<Camera>() != null)
		{
			yield return null;
			UnityEngine.Object.Destroy(base.GetComponent<Camera>());
		}
	}

	public void OnAddBombHoldBonus(int holderID, int amount)
	{
		PlayerCharacterManager playerCharacterManager = Players(holderID);
		if (playerCharacterManager != null)
		{
			playerStats[holderID].bombHoldBonusTotal += amount;
		}
	}

	public void OnAddBombDepositBonus(int holderID, int amount)
	{
		PlayerCharacterManager playerCharacterManager = Players(holderID);
		if (playerCharacterManager != null)
		{
			playerStats[holderID].bombDepositBonusTotal += amount;
		}
	}

	private void AddAssistBonuses(int shooterID, int victimID)
	{
		float val = 0.2f;
		ServiceManager.Instance.UpdateProperty("assist_bonus_per_percent", ref val);
		float val2 = 0.9f;
		ServiceManager.Instance.UpdateProperty("max_assist_percent", ref val2);
		foreach (KeyValuePair<int, PlayerStats> playerStat in playerStats)
		{
			if (playerStat.Value.playerTeam != playerStats[victimID].playerTeam && playerStat.Key != shooterID && playerStat.Value.CurrentAssistsDamage.ContainsKey(victimID) && playerStat.Value.CurrentAssistsDamage[victimID] > 0f)
			{
				int num = 0;
				float num2 = 100f;
				if (playerStats[victimID].playerLoadout.model.properties.ContainsKey("health"))
				{
					num2 = (float)playerStats[victimID].playerLoadout.model.properties["health"];
				}
				else
				{
					Debug.LogWarning("target has no health property!");
				}
				num = (int)(val * Mathf.Min(val2, playerStat.Value.CurrentAssistsDamage[victimID] / num2) * 100f);
				playerStat.Value.assistBonusTotal += num;
				playerStat.Value.CurrentAssistsDamage[victimID] = 0f;
				if (HUD.Instance != null && HUD.Instance.PlayerController != null && playerStat.Key == HUD.Instance.PlayerController.OwnerID)
				{
					HUD.Instance.OnAddAssistNotication("+" + string.Format("{0:#,0}", num));
				}
			}
		}
	}

	public void OnPlayerKilled(int shooterID, int victimID, bool isExplosion, bool isMelee, bool isHeadshot)
	{
		PlayerCharacterManager playerCharacterManager = Players(shooterID);
		PlayerCharacterManager playerCharacterManager2 = Players(victimID);
		if (playerCharacterManager == null || playerCharacterManager2 == null)
		{
			Debug.LogError("when killing a player, we should always have a shooter and a victim, shooterID: " + shooterID + "victim id: " + victimID);
			return;
		}
		playerStats[victimID].addDeath();
		playerStats[shooterID].addKillsCaused(victimID);
		AddAssistBonuses(shooterID, victimID);
		if (HUD.Instance != null && HUD.Instance.PlayerController != null && HUD.Instance.PlayerController.OwnerID == victimID)
		{
			HUD.Instance.lastKiller = playerStats[shooterID];
		}
		if (playerCharacterManager.team != playerCharacterManager2.team)
		{
			int val = 0;
			ServiceManager.Instance.UpdateProperty("kill_joule_bonus", ref val);
			int val2 = 3;
			ServiceManager.Instance.UpdateProperty("min_kills_for_streak", ref val2);
			float val3 = 0.1f;
			ServiceManager.Instance.UpdateProperty("kill_streak_bonus_ratio", ref val3);
			int val4 = 0;
			ServiceManager.Instance.UpdateProperty("kill_xp_bonus", ref val4);
			int num = 0;
			int num2 = 0;
			if (playerStats[victimID].currentKillStreak >= val2)
			{
				int val5 = 20;
				ServiceManager.Instance.UpdateProperty("streak_stop_base_bonus", ref val5);
				num2 = val5 * (playerStats[victimID].currentKillStreak - val2 + 1);
				playerStats[shooterID].stopStreakBonusTotal += num2;
			}
			playerStats[shooterID].currentKillStreak++;
			if (playerStats[shooterID].currentKillStreak > playerStats[shooterID].longestKillStreak)
			{
				playerStats[shooterID].longestKillStreak = playerStats[shooterID].currentKillStreak;
			}
			if (playerStats[shooterID].currentKillStreak >= val2)
			{
				num = (int)((float)val * (0.1f * (float)(playerStats[shooterID].currentKillStreak - val2 + 1)));
				playerStats[shooterID].killStreakBonusTotal += num;
			}
			playerStats[shooterID].CurrentAssistsDamage[victimID] = 0f;
			if (HUD.Instance != null && HUD.Instance.PlayerController != null && HUD.Instance.PlayerController.OwnerID == shooterID)
			{
				if (playerCharacterManager.playerLoadout.model.name == "Riggs" && playerCharacterManager2.playerLoadout.model.name == "Riggs")
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["POLAR_OPPOSITES"]);
				}
				if (Instance.playerStats[shooterID].NetKills == 10)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BEARFOOTED"]);
				}
				else if (Instance.playerStats[shooterID].NetKills == 15)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["PAWSIBLY_INSANE"]);
				}
				else if (Instance.playerStats[shooterID].NetKills == 20)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["LEAVE_NO_BEAR_BEHIND"]);
				}
				if (playerStats[shooterID].longestKillStreak == 5)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["KILL_STREAK_5"]);
				}
				else if (playerStats[shooterID].longestKillStreak == 10)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["KILL_STREAK_10"]);
				}
				else if (playerStats[shooterID].longestKillStreak == 15)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["KILL_STREAK_15"]);
				}
				CumulativeStats.Instance.numOpponentKills++;
				if (num2 > 0)
				{
					HUD.Instance.OnAddStopStreakNotification("+" + string.Format("{0:#,0}", num2));
				}
				if (num > 0)
				{
					HUD.Instance.OnAddKillStreakNotification(playerStats[shooterID].currentKillStreak.ToString(), "+" + string.Format("{0:#,0}", num));
				}
				HUD.Instance.OnAddRewardNotification("+" + string.Format("{0:#,0}", val), "+" + string.Format("{0:#,0}", val4));
			}
		}
		if (HUD.Instance != null)
		{
			if (shooterID == victimID)
			{
				HUD.Instance.OnSuicideNotification(playerCharacterManager.playerLoadout.model.name, playerCharacterManager.playerLoadout.skin.name, playerCharacterManager.team);
			}
			else
			{
				HUD.Instance.OnKillNotification(playerCharacterManager.playerLoadout.model.name, playerCharacterManager.playerLoadout.skin.name, playerCharacterManager.team, playerCharacterManager2.playerLoadout.model.name, playerCharacterManager2.playerLoadout.skin.name, playerCharacterManager2.team, isHeadshot);
			}
			if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
			{
				HUD.Instance.UpdateLeaderDisplay();
			}
		}
		playerStats[victimID].currentKillStreak = 0;
		if (isMelee)
		{
			if (playerCharacterManager.PlayerController != null)
			{
				playerCharacterManager.PlayerController.OnPlayMeleeKillSound();
			}
		}
		else if (playerCharacterManager.PlayerController != null)
		{
			playerCharacterManager.PlayerController.OnPlayRegularKillSound();
		}
		if (playerCharacterManager2.team == Team.RED || playerCharacterManager2.team == Team.BLUE)
		{
			Team team = ((shooterID == victimID) ? playerCharacterManager2.team : playerCharacterManager.team);
			if (shooterID != victimID)
			{
				teams[team].kills++;
			}
			else
			{
				teams[playerCharacterManager2.team].kills--;
			}
			if (_gameMode == GameMode.TB)
			{
				HUD.Instance.OnSetScore(team, teams[team].kills);
			}
		}
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL && playerCharacterManager2.OwnerID == localPlayerID)
		{
			ReportDeathTime(localPlayerID, PhotonManager.Instance.ServerTimeInMilliseconds);
		}
	}

	public void ReportDeathTime(int id, int deathTimeInMS)
	{
		if (deathTimeInMS >= PhotonManager.Instance.ServerTimeInMilliseconds + 1500)
		{
			return;
		}
		if (!_deathTimes.ContainsKey(id))
		{
			_deathTimes[id] = deathTimeInMS;
		}
		PlayerCharacterManager playerCharacterManager = LocalPlayerCharacterManager();
		if (_gameMode == GameMode.ROYL && (id == localPlayerID || (playerCharacterManager != null && playerCharacterManager.PlayerController != null && !playerCharacterManager.PlayerController.IsDead && _deathTimes.Count == playerStats.Count - 1)))
		{
			EndGame();
			if (LocalPlayerDiedButWontSubmitReport(id))
			{
				EventTracker.TrackEvent(MatchEventsHelper.MatchCompleted(Report.GenerateGameReport(this)));
			}
		}
		else
		{
			TryAddRadarForPlayers();
		}
	}

	private bool LocalPlayerDiedButWontSubmitReport(int id)
	{
		return id == localPlayerID && _deathTimes.Count < playerStats.Count - 1;
	}

	private void TryAddRadarForPlayers()
	{
		if (_gameMode == GameMode.ROYL && HUD.Instance != null && RoyaleRemainingPlayers() <= 4)
		{
			HUD.Instance.ForceAddRadarForRoyale();
		}
	}

	public void checkIfAllPlayersLeft()
	{
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, PlayerCharacterManager> player in players)
		{
			if (player.Value.team == Team.BLUE)
			{
				num++;
			}
			else if (player.Value.team == Team.RED)
			{
				num2++;
			}
		}
		if (num == 0)
		{
			PenalizeTeamForLeaving(Team.BLUE);
			EndGame();
		}
		else if (num2 == 0)
		{
			PenalizeTeamForLeaving(Team.RED);
			EndGame();
		}
	}

	private void PenalizeTeamForLeaving(Team team)
	{
		if (_gameMode != GameMode.ROYL)
		{
			teams[team].kills = -1000;
			if (_gameMode == GameMode.CTF)
			{
				teams[team].bombsDeposited = -1000;
			}
		}
	}

	public void OnPlayerHasLeft(int ownerID)
	{
		if (players.ContainsKey(ownerID))
		{
			playerStats[ownerID].hasLeft = true;
			players.Remove(ownerID);
		}
		if (!_deathTimes.ContainsKey(ownerID))
		{
			_deathTimes.Add(ownerID, -1);
		}
		if (!IsGameSubmitted)
		{
			checkIfAllPlayersLeft();
			TryAddRadarForPlayers();
		}
	}

	private void OnSentSuccess()
	{
		if (this != null)
		{
			StartCoroutine(delayedEndGame());
		}
	}

	private void OnSentFailure()
	{
		Debug.LogError("failed to send to report: " + ServiceManager.Instance.LastError);
		if (this != null)
		{
			StartCoroutine(delayedEndGame());
		}
	}

	private void TryUnlockKillCountAchievements()
	{
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BEARZERKER"], (float)CumulativeStats.Instance.numOpponentKills / 100f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["COME_AT_ME"], (float)CumulativeStats.Instance.numOpponentKills / 200f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["CANT_BEAR_IT"], (float)CumulativeStats.Instance.numOpponentKills / 300f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BETTER_HAVE_MY_HONEY"], (float)CumulativeStats.Instance.numOpponentKills / 400f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["GRIZZLY_SITUATION"], (float)CumulativeStats.Instance.numOpponentKills / 500f * 100f);
	}

	private void TryUnlockWinningAchievements(Team winningTeam)
	{
		if (teams[Team.BLUE].kills != -1000 && teams[Team.RED].kills != -1000)
		{
			if (winningTeam == Team.RED && teams[Team.RED].kills - teams[Team.BLUE].kills >= 20)
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["UNBEARLIEVABLE"]);
			}
			else if (winningTeam == Team.BLUE && teams[Team.BLUE].kills - teams[Team.RED].kills >= 20)
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["UNBEARLIEVABLE"]);
			}
		}
		if (HUD.Instance.PlayerController.CharacterManager != null)
		{
			if (HUD.Instance.PlayerController.CharacterManager.playerLoadout.model.name == "Oliver")
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["ATTENTION"]);
			}
			else if (HUD.Instance.PlayerController.CharacterManager.playerLoadout.model.name == "Riggs")
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["HEAVY_SET_GO"]);
			}
			else if (HUD.Instance.PlayerController.CharacterManager.playerLoadout.model.name == "Huggable")
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["HUGGED_TO_DEATH"]);
			}
			else if (HUD.Instance.PlayerController.CharacterManager.playerLoadout.model.name == "Tillman")
			{
				Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["PANDAMONIUM"]);
			}
		}
		CumulativeStats.Instance.numWins++;
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["JUST_A_SMALL_TOWN_BEAR"], (float)CumulativeStats.Instance.numWins / 5f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BATTLE_OF_THE_BEARS"], (float)CumulativeStats.Instance.numWins / 25f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["WINNER_WINNER_CHICKEN_DINNER"], (float)CumulativeStats.Instance.numWins / 50f * 100f);
	}

	private IEnumerator SubmitReport()
	{
		if (gameSubmitted)
		{
			yield break;
		}
		AdManager.Instance.FetchAd(LocalPlayerWon ? AdType.matchWon : AdType.matchLost);
		TryUnlockKillCountAchievements();
		gameSubmitted = true;
		foreach (KeyValuePair<int, PlayerCharacterManager> kvp in players)
		{
			if (kvp.Value != null && kvp.Value.PlayerController != null)
			{
				kvp.Value.PlayerController.canControl = false;
				kvp.Value.PlayerController.DamageReceiver.isInvincible = true;
			}
		}
		if (HUD.Instance != null && HUD.Instance.PlayerController != null)
		{
			int minKillsForStreak = 3;
			ServiceManager.Instance.UpdateProperty("min_kills_for_streak", ref minKillsForStreak);
			if (playerStats[HUD.Instance.PlayerController.OwnerID].longestKillStreak >= minKillsForStreak)
			{
				Bootloader.Instance.reportScore(GameCenterIDDictionaries.Leaderboards["KILL_STREAK"], playerStats[HUD.Instance.PlayerController.OwnerID].longestKillStreak);
			}
			if (_gameMode == GameMode.KOTH)
			{
				teams[Team.RED].pointsCaptured = KOTHManager.Instance.GetTeamScore(Team.RED);
				teams[Team.BLUE].pointsCaptured = KOTHManager.Instance.GetTeamScore(Team.BLUE);
			}
			yield return new WaitForSeconds(1.5f);
			Team winningTeam = (Team)WinningTeam.id;
			List<PlayerLoadout> winningLoadouts = new List<PlayerLoadout>();
			switch (_gameMode)
			{
			case GameMode.TB:
			case GameMode.CTF:
			case GameMode.KOTH:
				PopulateWinningLoadoutsTeamMatch(winningLoadouts, winningTeam);
				break;
			case GameMode.FFA:
			case GameMode.ROYL:
				PopulateWinningLoadoutsNonTeam(winningLoadouts);
				break;
			default:
				throw new Exception("No winning loadout method defined for game mode " + _gameMode);
			}
			if (winningTeam == HUD.Instance.PlayerController.Team)
			{
				TryUnlockWinningAchievements(winningTeam);
			}
			HUD.Instance.OnGameEnd(winningTeam, winningLoadouts);
		}
		else
		{
			yield return new WaitForSeconds(1.5f);
		}
		if (_gameMode == GameMode.KOTH)
		{
			teams[Team.RED].pointsCaptured = KOTHManager.Instance.GetTeamScore(Team.RED);
			teams[Team.BLUE].pointsCaptured = KOTHManager.Instance.GetTeamScore(Team.BLUE);
		}
		StartCoroutine(delayedSubmitReport());
	}

	private void PopulateWinningLoadoutsNonTeam(List<PlayerLoadout> winningLoadouts)
	{
		PlayerStats playerStats = ((_gameMode != 0) ? GetRoyaleWinner() : GetFFALeader());
		if (playerStats != null)
		{
			winningLoadouts.Add(playerStats.playerLoadout);
		}
		if (HUD.Instance.PlayerController != null)
		{
			if (playerStats != null && playerStats.id == HUD.Instance.PlayerController.OwnerID)
			{
				HUD.Instance.PlayerController.OnPlayVictorySound();
			}
			else
			{
				HUD.Instance.PlayerController.OnPlayDefeatSound();
			}
		}
		if (HUD.Instance != null)
		{
			HUD.Instance.enabled = false;
		}
		List<PlayerCharacterManager> playerCharacterManagers = Instance.GetPlayerCharacterManagers();
		foreach (PlayerCharacterManager item in playerCharacterManagers)
		{
			if (item.PlayerController != null)
			{
				item.PlayerController.enabled = false;
				if (item.PlayerController.Motor != null)
				{
					item.PlayerController.Motor.enabled = false;
				}
			}
			item.PlayerController.WeaponManager.OnStopFiring();
		}
	}

	private void PopulateWinningLoadoutsTeamMatch(List<PlayerLoadout> winningLoadouts, Team winningTeam)
	{
		foreach (KeyValuePair<int, PlayerCharacterManager> player in players)
		{
			if (!(player.Value != null) || !(player.Value.PlayerController != null))
			{
				continue;
			}
			if (player.Value.team == winningTeam)
			{
				if (player.Value.PlayerController.OwnerID == HUD.Instance.PlayerController.OwnerID)
				{
					player.Value.PlayerController.OnPlayVictorySound();
				}
				if (_gameMode != 0 || winningLoadouts.Count == 0)
				{
					winningLoadouts.Add(player.Value.playerLoadout);
				}
			}
			else if (player.Value.PlayerController.OwnerID == HUD.Instance.PlayerController.OwnerID)
			{
				player.Value.PlayerController.OnPlayDefeatSound();
			}
			if (player.Value.PlayerController != null)
			{
				player.Value.PlayerController.enabled = false;
				if (player.Value.PlayerController.Motor != null)
				{
					player.Value.PlayerController.Motor.enabled = false;
				}
			}
			if (HUD.Instance != null)
			{
				HUD.Instance.enabled = false;
			}
			player.Value.PlayerController.WeaponManager.OnStopFiring();
		}
	}

	private IEnumerator delayedSubmitReport()
	{
		yield return new WaitForSeconds(2.5f);
		if (_gameMode == GameMode.ROYL)
		{
			if (_deathTimes.Count >= playerStats.Count - 1)
			{
				ServiceManager.Instance.SendReport(Report.GenerateGameReport(this), null, null);
			}
			yield return new WaitForSeconds(2f);
			PhotonManager.Instance.Leave();
			Application.LoadLevel("MainMenu");
		}
		else
		{
			ServiceManager.Instance.SendReport(Report.GenerateGameReport(this), OnSentSuccess, OnSentFailure);
		}
	}

	private IEnumerator delayedEndGame()
	{
		if (Preferences.Instance.AdsEnabled)
		{
			TryShowEndGameAd();
		}
		yield return new WaitForSeconds(2f);
		PhotonManager.Instance.Leave();
		Application.LoadLevel("ReportScreen");
	}

	private void TryShowEndGameAd()
	{
		if (_gameMode != GameMode.ROYL)
		{
			AdManager.Instance.ShowAd(LocalPlayerWon ? AdType.matchWon : AdType.matchLost);
		}
	}

	public void OnRedKilled()
	{
		teams[Team.BLUE].kills++;
		if (_gameMode == GameMode.TB)
		{
			HUD.Instance.OnSetScore(Team.BLUE, teams[Team.BLUE].kills);
		}
	}

	public void OnBlueKilled()
	{
		teams[Team.RED].kills++;
		if (_gameMode == GameMode.TB)
		{
			HUD.Instance.OnSetScore(Team.RED, teams[Team.RED].kills);
		}
	}

	public void OnBombDeposited(Team team)
	{
		teams[team].bombsDeposited++;
		if (_gameMode == GameMode.CTF)
		{
			HUD.Instance.OnSetScore(team, teams[team].bombsDeposited);
		}
	}

	public virtual void TimeUpdate(int newServerTime)
	{
		if (gameStartTime == 0 || gameLength_ms == 0 || gameSubmitted)
		{
			return;
		}
		int num = currentServerTime / 1000;
		currentServerTime = newServerTime;
		int num2 = currentServerTime / 1000;
		int num3 = EndTime / 1000;
		int num4 = num3 - num;
		int num5 = num3 - num2;
		if (HUD.Instance != null)
		{
			if (num4 > 241 && num5 <= 241)
			{
				HUD.Instance.OnAddTimeNotification();
			}
			if (num4 > 181 && num5 <= 181)
			{
				HUD.Instance.OnAddTimeNotification();
			}
			if (num4 > 121 && num5 <= 121)
			{
				HUD.Instance.OnAddTimeNotification();
				HUD.Instance.OnPlaySound(HUD.Instance.twoMinuteWarning);
			}
			if (num4 > 61 && num5 <= 61)
			{
				HUD.Instance.OnAddTimeNotification();
				HUD.Instance.OnPlaySound(HUD.Instance.oneMinuteWarning);
			}
			if (num4 > 31 && num5 <= 31)
			{
				HUD.Instance.OnAddTimeNotification();
				HUD.Instance.OnPlaySound(HUD.Instance.thirtySecondWarning);
			}
			if (num4 > 11 && num5 <= 11)
			{
				HUD.Instance.OnAddTimeNotification();
				HUD.Instance.OnPlaySound(HUD.Instance.tenSecondWarning);
			}
			if (num4 > 6 && num5 <= 6)
			{
				HUD.Instance.OnPlaySound(HUD.Instance.fiveSecondWarning);
			}
			if (num4 > 5 && num5 <= 5)
			{
				HUD.Instance.OnPlaySound(HUD.Instance.fourSecondWarning);
			}
			if (num4 > 4 && num5 <= 4)
			{
				HUD.Instance.OnPlaySound(HUD.Instance.threeSecondWarning);
			}
			if (num4 > 3 && num5 <= 3)
			{
				HUD.Instance.OnPlaySound(HUD.Instance.twoSecondWarning);
			}
			if (num4 > 2 && num5 <= 2)
			{
				HUD.Instance.OnPlaySound(HUD.Instance.oneSecondWarning);
			}
		}
		if (num2 >= num3 - 1)
		{
			EndGame();
		}
	}
}
