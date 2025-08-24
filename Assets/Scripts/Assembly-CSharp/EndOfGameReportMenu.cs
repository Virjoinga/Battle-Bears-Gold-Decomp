using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Analytics;
using Prime31;
using UnityEngine;
using Utils.Comparers;

public class EndOfGameReportMenu : MonoBehaviour
{
	public GameObject fetchingSpinner;

	public TextMesh fetchingStatsText;

	public GameObject redWinsTitle;

	public GameObject blueWinsTitle;

	public TextMesh[] blueLivesIndicators;

	public TextMesh[] redLivesIndicators;

	public Transform[] redPlayerDisplays;

	public Transform[] bluePlayerDisplays;

	public TextMesh ourPlayerName;

	public TextMesh ourKills;

	public TextMesh ourDeaths;

	public TextMesh ourRank;

	public TextMesh matchTime;

	public TextMesh skillChange;

	public TextMesh finalSkill;

	public TextMesh matchXP;

	public TextMesh killXP;

	public TextMesh bonusXP;

	public TextMesh extraXP;

	public TextMesh totalXP;

	public TextMesh matchJoules;

	public TextMesh killJoules;

	public TextMesh mapJoules;

	public TextMesh bonusJoules;

	public TextMesh totalJoules;

	public Transform rankIconMount;

	public Transform ourIconMount;

	public TextMesh redTotalDamage;

	public TextMesh blueTotalDamage;

	public TextMesh levelIndicator;

	public ReportMenuController praisePopup;

	public ReportMenuController reportPopup;

	public GameObject loadingMainMenuPopup;

	public GameObject errorPopup;

	public TextMesh errorPopupText;

	private Report.GameReport gameReport;

	private GameResults gameResults;

	private Animation myAnimation;

	private GUIController guiController;

	private Report.Player ourPlayer;

	public Material enoughEnergyFont;

	public Material notEnoughEnergyFont;

	private List<Report.Player> allPlayers = new List<Report.Player>();

	private List<Report.Player> bluePlayers;

	private List<Report.Player> redPlayers;

	public AudioClip[] clickSounds;

	public GameObject buyGasPopup;

	public Transform popupRoot;

	public GameObject facebookRoot;

	private bool isFacebookPosting;

	private Camera popupCamera;

	public TextMesh firstSkillLine;

	public TextMesh secondSkillLine;

	private bool isViewingPlayerStats;

	private void OnEnable()
	{
		FacebookManager.sessionOpenedEvent += facebookLogin;
		FacebookManager.loginFailedEvent += facebookLoginFailed;
		FacebookManager.dialogCompletedWithUrlEvent += facebookPost;
		FacebookManager.dialogFailedEvent += facebookPostFailed;
	}

	private void OnDisable()
	{
		FacebookManager.sessionOpenedEvent -= facebookLogin;
		FacebookManager.loginFailedEvent -= facebookLoginFailed;
		FacebookManager.dialogCompletedWithUrlEvent -= facebookPost;
		FacebookManager.dialogFailedEvent -= facebookPostFailed;
	}

	private void Awake()
	{
		myAnimation = base.animation;
		guiController = GetComponent(typeof(GUIController)) as GUIController;
		popupCamera = popupRoot.GetComponentInChildren<Camera>();
	}

	private void Start()
	{
		StartCoroutine(switchView(false));
		CumulativeStats.Instance.numGamesPlayed++;
		CumulativeStats.Instance.OnSaveStats();
		loadingMainMenuPopup.SetActive(false);
		errorPopup.SetActive(false);
		StartCoroutine(waitForPlayerStats());
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.player.ClearBBRProperties();
			PhotonNetwork.Disconnect();
		}
	}

	private void Update()
	{
		if (popupRoot.childCount < 2 && Input.GetKeyDown(KeyCode.Escape))
		{
			previousMenu();
		}
	}

	private IEnumerator switchView(bool viewPlayerStats)
	{
		guiController.IsActive = false;
		isViewingPlayerStats = viewPlayerStats;
		if (viewPlayerStats)
		{
			myAnimation.Play("PlayerStats");
			yield return new WaitForSeconds(myAnimation["PlayerStats"].length);
		}
		else
		{
			myAnimation.Play("TeamStats");
			yield return new WaitForSeconds(myAnimation["TeamStats"].length);
		}
		guiController.IsActive = true;
	}

	private void showReportResults()
	{
		if (Preferences.Instance.CurrentGameMode != 0)
		{
			redWinsTitle.SetActive(gameReport.summary.win_team == 0);
			blueWinsTitle.SetActive(gameReport.summary.win_team == 1);
		}
		TextMesh[] array = redLivesIndicators;
		foreach (TextMesh textMesh in array)
		{
			if (gameReport.summary.gameMode == "FFA")
			{
				if (gameReport.summary.teams[0].kills == -1000)
				{
					textMesh.text = "-";
				}
				else
				{
					textMesh.text = gameReport.summary.teams[0].kills.ToString();
				}
			}
			else if (gameReport.summary.gameMode == "CTF")
			{
				if (gameReport.summary.teams[0].bombsDeposited == -1000)
				{
					textMesh.text = "-";
				}
				else
				{
					textMesh.text = gameReport.summary.teams[0].bombsDeposited.ToString();
				}
			}
			else if (!(gameReport.summary.gameMode == "EBFT") && gameReport.summary.gameMode == "KTH")
			{
				if (gameReport.summary.teams[0].pointsCaptured == -1000)
				{
					textMesh.text = "-";
				}
				else
				{
					textMesh.text = gameReport.summary.teams[0].pointsCaptured.ToString();
				}
			}
		}
		TextMesh[] array2 = blueLivesIndicators;
		foreach (TextMesh textMesh2 in array2)
		{
			if (gameReport.summary.gameMode == "FFA")
			{
				if (gameReport.summary.teams[1].kills == -1000)
				{
					textMesh2.text = "-";
				}
				else
				{
					textMesh2.text = gameReport.summary.teams[1].kills.ToString();
				}
			}
			else if (gameReport.summary.gameMode == "CTF")
			{
				if (gameReport.summary.teams[1].bombsDeposited == -1000)
				{
					textMesh2.text = "-";
				}
				else
				{
					textMesh2.text = gameReport.summary.teams[1].bombsDeposited.ToString();
				}
			}
			else if (!(gameReport.summary.gameMode == "EBFT") && gameReport.summary.gameMode == "KTH")
			{
				if (gameReport.summary.teams[1].pointsCaptured == -1000)
				{
					textMesh2.text = "-";
				}
				else
				{
					textMesh2.text = gameReport.summary.teams[1].pointsCaptured.ToString();
				}
			}
		}
		bluePlayers = new List<Report.Player>();
		redPlayers = new List<Report.Player>();
		float num = 0f;
		float num2 = 0f;
		foreach (Report.Player player in gameReport.players)
		{
			if (player.team_id == 0)
			{
				redPlayers.Add(player);
				num += player.TotalDamageDealt;
			}
			else
			{
				bluePlayers.Add(player);
				num2 += player.TotalDamageDealt;
			}
		}
		blueTotalDamage.text = string.Format("{0:#,0}", (int)num2);
		redTotalDamage.text = string.Format("{0:#,0}", (int)num);
		allPlayers = new List<Report.Player>(gameReport.players);
		TeamModePlayerComparer comparer = new TeamModePlayerComparer();
		if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			allPlayers.Sort(new FFAPlayerReportComparer());
		}
		else
		{
			allPlayers.Sort(comparer);
		}
		bluePlayers.Sort(comparer);
		redPlayers.Sort(comparer);
		if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			SetupMenuForFFAGame(allPlayers);
			DisableIrrelevantReportButtonsForFFA();
		}
		else if (Preferences.Instance.CurrentGameMode == GameMode.TB || Preferences.Instance.CurrentGameMode == GameMode.CTF || Preferences.Instance.CurrentGameMode == GameMode.KOTH)
		{
			SetupMenuForTeamGame(redPlayers, bluePlayers);
			DisableIrrelevantReportButtonsForTeams();
		}
	}

	private List<Transform> GetAllPlayerSlots()
	{
		List<Transform> list = new List<Transform>();
		list.AddRange(redPlayerDisplays);
		list.AddRange(bluePlayerDisplays);
		return list;
	}

	private void SetupMenuForFFAGame(List<Report.Player> allPlayers)
	{
		List<Transform> allPlayerSlots = GetAllPlayerSlots();
		for (int i = 0; i < allPlayerSlots.Count; i++)
		{
			Transform transform = allPlayerSlots[i];
			if (i < allPlayers.Count)
			{
				Report.Player player = allPlayers[i];
				(transform.Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = player.greeName;
				(transform.Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = player.TotalKills.ToString();
				(transform.Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = player.TotalDeaths.ToString();
				(transform.Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = player.id.ToString();
				transform.Find("rankLevel/level").GetComponent<TextMesh>().text = player.level.ToString();
				Rank rank = ServiceManager.GetRank(player.skill);
				UnityEngine.Object @object = Resources.Load("Icons/Rank/" + rank);
				if (@object != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
					gameObject.transform.parent = transform.Find("rankLevel/rank_mount");
					gameObject.transform.localEulerAngles = Vector3.zero;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					gameObject.layer = LayerMask.NameToLayer("Menu");
				}
				string text = ServiceManager.Instance.GetItemByID(player.loadout.model).name;
				string text2 = ServiceManager.Instance.GetItemByID(player.loadout.skin).name;
				string text3 = ((player.id != GameManager.Instance.localPlayerID) ? "_blue" : "_red");
				UnityEngine.Object object2 = Resources.Load("Icons/Characters/" + text + "/" + text2 + text3);
				if (object2 != null)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(object2) as GameObject;
					gameObject2.transform.parent = transform.transform.Find("mount");
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localEulerAngles = Vector3.zero;
					gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			else
			{
				transform.Find("name").GetComponent<TextMesh>().text = string.Empty;
				transform.Find("kills").GetComponent<TextMesh>().text = string.Empty;
				transform.Find("deaths").GetComponent<TextMesh>().text = string.Empty;
				transform.Find("playerId").GetComponent<TextMesh>().text = string.Empty;
				transform.Find("rankLevel/level").GetComponent<TextMesh>().text = string.Empty;
			}
		}
	}

	private void SetupMenuForTeamGame(List<Report.Player> redPlayers, List<Report.Player> bluePlayers)
	{
		for (int i = 0; i < bluePlayerDisplays.Length; i++)
		{
			if (i >= bluePlayers.Count)
			{
				bluePlayerDisplays[i].Find("name").GetComponent<TextMesh>().text = string.Empty;
				bluePlayerDisplays[i].Find("kills").GetComponent<TextMesh>().text = string.Empty;
				bluePlayerDisplays[i].Find("deaths").GetComponent<TextMesh>().text = string.Empty;
				bluePlayerDisplays[i].Find("rankLevel/level").GetComponent<TextMesh>().text = string.Empty;
				(bluePlayerDisplays[i].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				continue;
			}
			(bluePlayerDisplays[i].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].greeName;
			(bluePlayerDisplays[i].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].TotalKills.ToString();
			(bluePlayerDisplays[i].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].TotalDeaths.ToString();
			bluePlayerDisplays[i].Find("rankLevel/level").GetComponent<TextMesh>().text = bluePlayers[i].level.ToString();
			(bluePlayerDisplays[i].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = bluePlayers[i].id.ToString();
			Rank rank = ServiceManager.GetRank(bluePlayers[i].skill);
			UnityEngine.Object @object = Resources.Load("Icons/Rank/" + rank);
			if (@object != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = bluePlayerDisplays[i].Find("rankLevel/rank_mount");
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.layer = LayerMask.NameToLayer("Menu");
			}
			string text = ServiceManager.Instance.GetItemByID(bluePlayers[i].loadout.model).name;
			string text2 = ServiceManager.Instance.GetItemByID(bluePlayers[i].loadout.skin).name;
			UnityEngine.Object object2 = Resources.Load("Icons/Characters/" + text + "/" + text2 + "_blue");
			if (object2 != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(object2) as GameObject;
				gameObject2.transform.parent = bluePlayerDisplays[i].transform.Find("mount");
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localEulerAngles = Vector3.zero;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		for (int j = 0; j < redPlayerDisplays.Length; j++)
		{
			if (j >= redPlayers.Count)
			{
				(redPlayerDisplays[j].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(redPlayerDisplays[j].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				(redPlayerDisplays[j].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				redPlayerDisplays[j].Find("rankLevel/level").GetComponent<TextMesh>().text = string.Empty;
				(redPlayerDisplays[j].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Empty;
				continue;
			}
			(redPlayerDisplays[j].Find("name").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].greeName;
			(redPlayerDisplays[j].Find("kills").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].TotalKills.ToString();
			(redPlayerDisplays[j].Find("deaths").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].TotalDeaths.ToString();
			redPlayerDisplays[j].Find("rankLevel/level").GetComponent<TextMesh>().text = redPlayers[j].level.ToString();
			(redPlayerDisplays[j].Find("playerId").GetComponent(typeof(TextMesh)) as TextMesh).text = redPlayers[j].id.ToString();
			Rank rank2 = ServiceManager.GetRank(redPlayers[j].skill);
			UnityEngine.Object object3 = Resources.Load("Icons/Rank/" + rank2);
			if (object3 != null)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(object3) as GameObject;
				gameObject3.transform.parent = redPlayerDisplays[j].Find("rankLevel/rank_mount");
				gameObject3.transform.localEulerAngles = Vector3.zero;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject3.layer = LayerMask.NameToLayer("Menu");
			}
			string text3 = ServiceManager.Instance.GetItemByID(redPlayers[j].loadout.model).name;
			string text4 = ServiceManager.Instance.GetItemByID(redPlayers[j].loadout.skin).name;
			UnityEngine.Object object4 = Resources.Load("Icons/Characters/" + text3 + "/" + text4 + "_red");
			if (object4 != null)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate(object4) as GameObject;
				gameObject4.transform.parent = redPlayerDisplays[j].transform.Find("mount");
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localEulerAngles = Vector3.zero;
				gameObject4.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	private IEnumerator waitForPlayerStats()
	{
		yield return new WaitForSeconds(0.1f);
		gameReport = ServiceManager.Instance.LastGameReport;
		showReportResults();
		while (ServiceManager.Instance.WaitingForResults)
		{
			yield return new WaitForSeconds(0.25f);
		}
		fetchingSpinner.gameObject.SetActive(false);
		fetchingStatsText.text = "PLAYER STATS";
		EventTracker.TrackEvent(MatchEventsHelper.MatchCompleted(gameReport));
		gameResults = ServiceManager.Instance.LastGameResults;
		ServiceManager.Instance.RefreshPlayerStats(OnStatsUpdated, null);
		showGameResults();
		if (gameResults.errorString != null && gameResults.errorString != string.Empty)
		{
			StartCoroutine(showGameError(gameResults.errorString));
		}
	}

	private IEnumerator showGameError(string reason)
	{
		errorPopup.SetActive(true);
		errorPopupText.text = reason;
		yield return new WaitForSeconds(5f);
		errorPopup.SetActive(false);
	}

	private void OnStatsUpdated()
	{
		levelIndicator.text = ((int)ServiceManager.Instance.GetStats().level).ToString();
		double skill = ServiceManager.Instance.GetStats().skill;
		Bootloader.Instance.reportScore(GameCenterIDDictionaries.Leaderboards["RANK"], skill);
	}

	private void showGameResults()
	{
		foreach (Report.Player player in gameReport.players)
		{
			if (player.id == ServiceManager.Instance.GetStats().pid)
			{
				ourPlayer = player;
				break;
			}
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("playerId", ourPlayer.id);
		dictionary.Add("kills", ourPlayer.TotalKills);
		dictionary.Add("deaths", ourPlayer.TotalDeaths);
		dictionary.Add("longestKillStreak", ourPlayer.longestKillStreak);
		dictionary.Add("win", GameManager.Instance.WinningTeam.id == ourPlayer.team_id);
		if (ourPlayer != null)
		{
			if (ourPlayer.team_id == 0)
			{
				ourPlayerName.text = ourPlayer.greeName;
			}
			else
			{
				ourPlayerName.text = ourPlayer.greeName;
			}
			ourKills.text = ourPlayer.TotalKills.ToString();
			ourDeaths.text = ourPlayer.TotalDeaths.ToString();
			ourRank.text = (allPlayers.IndexOf(ourPlayer) + 1).ToString();
			string text = ServiceManager.Instance.GetItemByID(ourPlayer.loadout.model).name;
			string text2 = ServiceManager.Instance.GetItemByID(ourPlayer.loadout.skin).name;
			string empty = string.Empty;
			empty = ((ourPlayer.team_id != 0) ? "_blue" : "_red");
			UnityEngine.Object @object = Resources.Load("Icons/Characters/" + text + "/" + text2 + empty);
			if (@object != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = ourIconMount;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		int num = ((int)gameReport.summary.match_end - (int)gameReport.summary.match_start) / 1000;
		int num2 = num / 60;
		int num3 = num % 60;
		if (num3 < 10)
		{
			matchTime.text = num2 + ":0" + num3;
		}
		else
		{
			matchTime.text = num2 + ":" + num3;
		}
		bool val = true;
		ServiceManager.Instance.UpdateProperty("display_rank", ref val);
		skillChange.renderer.enabled = val;
		finalSkill.renderer.enabled = val;
		if (!val)
		{
			firstSkillLine.text = string.Empty;
			secondSkillLine.text = "Current Skill Rank";
		}
		if (gameResults.SkillDelta <= 0f)
		{
			skillChange.text = ((int)gameResults.SkillDelta).ToString();
		}
		else
		{
			skillChange.text = "+" + (int)gameResults.SkillDelta;
		}
		if (gameResults.newSkill >= 0f)
		{
			finalSkill.text = ((int)gameResults.newSkill).ToString();
		}
		else
		{
			finalSkill.text = "Error";
		}
		Rank rank = ServiceManager.GetRank(gameResults.oldSkill);
		Rank rank2 = ServiceManager.GetRank(gameResults.newSkill);
		if (rank != rank2)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("old_rank", Enum.GetName(typeof(Rank), (int)rank));
			dictionary2.Add("new_rank", Enum.GetName(typeof(Rank), (int)rank2));
			if (!(gameResults.oldSkill < gameResults.newSkill))
			{
			}
		}
		UnityEngine.Object object2 = Resources.Load("Icons/Rank/" + rank2);
		if (object2 != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(object2) as GameObject;
			gameObject2.transform.parent = rankIconMount;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = LayerMask.NameToLayer("Menu");
		}
		if (gameResults.baseXP >= 0)
		{
			matchXP.text = gameResults.baseXP.ToString();
		}
		else
		{
			matchXP.text = "Error";
		}
		if (gameResults.killXP >= 0)
		{
			killXP.text = gameResults.killXP.ToString();
		}
		else
		{
			killXP.text = "Error";
		}
		extraXP.text = string.Empty;
		int num4 = gameResults.baseXP + gameResults.killXP + gameResults.extraXP;
		if (num4 >= 0)
		{
			totalXP.text = num4.ToString();
		}
		else
		{
			totalXP.text = "Error";
		}
		if (gameResults.baseJoules >= 0)
		{
			matchJoules.text = gameResults.baseJoules.ToString();
		}
		else
		{
			matchJoules.text = "Error";
		}
		if (gameResults.killJoules >= 0)
		{
			killJoules.text = gameResults.killJoules.ToString();
		}
		else
		{
			killJoules.text = "Error";
		}
		if (gameResults.mapJoules >= 0)
		{
			mapJoules.text = gameResults.mapJoules.ToString();
		}
		else
		{
			mapJoules.text = "Error";
		}
		if (gameResults.bonusJoules >= 0)
		{
			bonusJoules.text = gameResults.bonusJoules.ToString();
		}
		else
		{
			bonusJoules.text = "Error";
		}
		int num5 = gameResults.baseJoules + gameResults.killJoules + gameResults.mapJoules + gameResults.bonusJoules;
		if (num5 >= 0)
		{
			totalJoules.text = num5.ToString();
		}
		else
		{
			totalJoules.text = "Error";
		}
		AdjustFontColours(gameResults.enoughEnergy);
		LogCollectedJoules();
		LogBonusJoules();
	}

	private void AdjustFontColours(bool b)
	{
		matchXP.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		killXP.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		totalXP.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		matchJoules.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		killJoules.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		mapJoules.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		bonusJoules.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
		totalJoules.renderer.material = ((!b) ? notEnoughEnergyFont : enoughEnergyFont);
	}

	private void LogCollectedJoules()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		int result = 0;
		int.TryParse(mapJoules.text, out result);
		dictionary.Add("amount", result);
	}

	private void LogBonusJoules()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("amount", gameResults.bonusJoules);
		dictionary.Add("bear", ServiceManager.Instance.GetItemByID(ourPlayer.loadout.model).name);
		dictionary.Add("map_name", ServiceManager.Instance.LastGameReport.MapName);
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[UnityEngine.Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "playerStats_btn":
			if (!ServiceManager.Instance.WaitingForResults)
			{
				StartCoroutine(switchView(true));
			}
			break;
		case "mainMenu_btn":
			loadingMainMenuPopup.SetActive(false);
			Application.LoadLevel("PostgameAdScreen");
			break;
		case "teamStats_btn":
			StartCoroutine(switchView(false));
			break;
		case "fb_btn":
			facebookRoot.animation.Play();
			if (!isFacebookPosting)
			{
				isFacebookPosting = true;
				if (!FacebookAndroid.isSessionValid())
				{
					FacebookAndroid.login();
				}
				else
				{
					postToFacebook();
				}
			}
			break;
		case "gas_btn":
			createPopup(buyGasPopup);
			break;
		default:
			CheckForReportButtonClick(b);
			break;
		}
	}

	private void DisableIrrelevantReportButtonsForFFA()
	{
		int num = 4;
		for (int num2 = 8; num2 > allPlayers.Count; num2--)
		{
			if (num2 > num)
			{
				DisableThumbsUpAndDownButton("Blue" + (num2 - 4));
			}
			else
			{
				DisableThumbsUpAndDownButton("Red" + num2);
			}
		}
		for (int i = 1; i <= allPlayers.Count; i++)
		{
			if (allPlayers[i - 1].id == GameManager.Instance.localPlayerID)
			{
				if (i > num)
				{
					DisableThumbsUpAndDownButton("Blue" + (i - 4));
				}
				else
				{
					DisableThumbsUpAndDownButton("Red" + i);
				}
			}
		}
	}

	private void DisableIrrelevantReportButtonsForTeams()
	{
		DisableReportButtons(redPlayers, "Red");
		DisableReportButtons(bluePlayers, "Blue");
	}

	private void DisableReportButtons(List<Report.Player> players, string teamName)
	{
		int num = 4;
		for (int num2 = num; num2 > players.Count; num2--)
		{
			DisableThumbsUpAndDownButton(teamName + num2);
		}
		for (int i = 0; i < players.Count; i++)
		{
			Report.Player player = players[i];
			if (player.id == ServiceManager.Instance.GetStats().pid)
			{
				DisableThumbsUpAndDownButton(teamName + (i + 1));
			}
		}
	}

	private void DisableThumbsUpAndDownButton(string buttonName)
	{
		GameObject gameObject = GameObject.Find("thumbsUp" + buttonName);
		if (gameObject != null)
		{
			gameObject.gameObject.SetActive(false);
		}
		gameObject = GameObject.Find("thumbsDown" + buttonName);
		if (gameObject != null)
		{
			gameObject.gameObject.SetActive(false);
		}
	}

	private void CheckForReportButtonClick(GUIButton button)
	{
		string text = button.name;
		if (!text.Contains("thumbs"))
		{
			return;
		}
		string s = Regex.Replace(text, "[^0-9]", string.Empty);
		int num = int.Parse(s);
		int num2 = -1;
		List<Report.Player> list = null;
		if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			num += (text.Contains("Blue") ? 4 : 0);
			list = allPlayers;
		}
		else
		{
			Team team = (text.Contains("Blue") ? Team.BLUE : Team.RED);
			list = ((team != Team.BLUE) ? redPlayers : bluePlayers);
		}
		if (list == null)
		{
			return;
		}
		for (int i = 1; i <= list.Count; i++)
		{
			if (i == num)
			{
				num2 = list[i - 1].id;
				break;
			}
		}
		if (num2 != -1 && num2 != ServiceManager.Instance.GetStats().pid)
		{
			if (text.Contains("Down"))
			{
				OpenReportPopup(button, num2);
			}
			else if (text.Contains("Up"))
			{
				OpenPraisePopup(button, num2);
			}
		}
	}

	private void OpenReportPopup(GUIButton button, int playerID)
	{
		reportPopup.ShowMenu(playerID, button);
	}

	private void OpenPraisePopup(GUIButton button, int playerID)
	{
		praisePopup.ShowMenu(playerID, button);
	}

	private void facebookLogin()
	{
		postToFacebook();
	}

	private void MetroFacebookLogin(string error)
	{
		if (error == null)
		{
			postToFacebook();
		}
	}

	private void postToFacebook()
	{
		string empty = string.Empty;
		int num = allPlayers.IndexOf(ourPlayer) + 1;
		string empty2 = string.Empty;
		switch (num)
		{
		case 1:
			empty2 = "st";
			break;
		case 2:
			empty2 = "nd";
			break;
		case 3:
			empty2 = "rd";
			break;
		default:
			empty2 = "th";
			break;
		}
		string text = "s";
		if (ourPlayer.TotalKills == 1 || ourPlayer.TotalKills == -1)
		{
			text = string.Empty;
		}
		empty = "I just got " + num + empty2 + " place and " + ourPlayer.TotalKills + " kill" + text + " in BATTLE BEARS GOLD!";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("description", empty);
		dictionary.Add("link", "https://play.google.com/store/apps/details?id=net.skyvu.battlebearsgold&hl=en");
		dictionary.Add("name", "BATTLE BEARS!");
		dictionary.Add("picture", "https://battlebears.com/wp-content/uploads/2013/04/BBG_Icon.png");
		dictionary.Add("caption", "The craziest mobile multiplayer shooter");
		Dictionary<string, object> parameters = dictionary;
		FacebookAndroid.showFacebookShareDialog(parameters);
	}

	private void facebookLoginFailed(P31Error error)
	{
		Debug.LogError("Facebook login failed from end of game: " + error.message);
	}

	private void facebookPost(string result)
	{
	}

	private void facebookPost()
	{
	}

	private void facebookPostFailed(P31Error error)
	{
	}

	private void createPopup(GameObject popup)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(popup) as GameObject;
		gameObject.transform.parent = popupRoot;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		Popup popup2 = gameObject.GetComponent(typeof(Popup)) as Popup;
		if (popup2 != null)
		{
			popup2.OnSetCallingObject(base.gameObject, popupCamera);
			popup2.OnSetGUIControllerToDisable(base.gameObject.GetComponent<GUIController>());
		}
	}

	private void previousMenu()
	{
		if (!isViewingPlayerStats)
		{
			StartCoroutine(switchView(!isViewingPlayerStats));
			return;
		}
		loadingMainMenuPopup.SetActive(false);
		Application.LoadLevel("PostgameAdScreen");
	}
}
