using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
	private const string _radarProModePurchaseName = "radar";

	private const string _jumpProModePurchaseName = "jump";

	private string _waitingForServer;

	private static HUD instance;

	private PlayerController playerController;

	private Transform playerCamera;

	public Camera hudCamera;

	public string blueKills;

	public string redKills;

	private int _maxClipStrLength = 6;

	private string _clipSize;

	private string _currentAmmo;

	private float _healthPercentage;

	public TextMesh fpsText;

	public TextMesh pingText;

	public bool hasPressedButton;

	public Transform iconMount;

	public Transform primaryMount;

	public Transform secondaryMount;

	public Transform meleeMount;

	public Transform specialMount;

	public Animation weaponSwitchAnimator;

	private bool isUsingPrimary = true;

	private float currentSpecialItemCooldownTime;

	public Transform weaponCooldownbar;

	private GameObject weaponCooldownbarObject;

	private float nextReloadEndTime;

	private float reloadDuration;

	public GameObject primaryReticle;

	public GameObject secondaryReticle;

	public GameObject weaponReloadOverlay;

	public GameObject abilityChargingOverlay;

	public Transform notificationsRoot;

	public GameObject healthNotification;

	public GameObject chickenNotification;

	public GameObject powerupNotification;

	public GameObject shieldNotification;

	public GameObject joulesNotification;

	public GameObject timeNotification;

	public GameObject killNotification;

	public GameObject speakNotification;

	public GameObject headshotNotification;

	public GameObject rewardNotification;

	public GameObject killStreakNotification;

	public GameObject stopStreakNotification;

	public GameObject assistNotification;

	public GameObject suicideNotification;

	public GameObject bombHoldNotification;

	public GameObject bombDepositNotification;

	public float notificationPushAmount = 20f;

	private bool isHuggable;

	public GameObject currentTeamspeakOverlay;

	public GameObject teamspeakOverlayPrefab;

	public float teamspeakDelay = 3f;

	[HideInInspector]
	public GameObject pauseMenu;

	[HideInInspector]
	public GameObject currentPauseMenu;

	public GUIController guiController;

	public GameObject mogaTutorialPopup;

	public GameObject endGamePopup;

	[SerializeField]
	private GameObject _royaleEndGamePopup;

	public GameObject lobbyCamera;

	public GameObject teamStatsOverlay;

	public GameObject ffaStatsOverlay;

	[HideInInspector]
	public GameObject currentStatsOverlay;

	public GameObject blueTeamMessage;

	public GameObject redTeamMessage;

	public GameObject mainTimeNotification;

	public Transform mainTimerMount;

	public AudioClip twoMinuteWarning;

	public AudioClip oneMinuteWarning;

	public AudioClip thirtySecondWarning;

	public AudioClip tenSecondWarning;

	public AudioClip fiveSecondWarning;

	public AudioClip fourSecondWarning;

	public AudioClip threeSecondWarning;

	public AudioClip twoSecondWarning;

	public AudioClip oneSecondWarning;

	public AudioClip endOfGameSound;

	public AudioClip sniperWarningSound;

	[HideInInspector]
	public bool isReloadAllowed = true;

	[HideInInspector]
	public bool isSpecialAllowed = true;

	[HideInInspector]
	public PlayerStats lastKiller;

	public bool isShootingMode;

	public Transform arrowIndicator;

	public TextMesh bombTextMesh;

	public GameObject ctfTutorialDisplay;

	private List<GameObject> currentIcons = new List<GameObject>();

	[SerializeField]
	private TextMesh _FFAKills;

	[SerializeField]
	private TextMesh _FFALeaderName;

	[SerializeField]
	private Transform _iconMount;

	private GameObject _currentFFALeaderIcon;

	private PlayerStats _previousLeader;

	[SerializeField]
	private RadarGuiElement _radarGuiElement;

	private bool _jumpButtonEnabled = true;

	public Transform PlayerCamera
	{
		get
		{
			return playerCamera;
		}
		set
		{
			playerCamera = value;
		}
	}

	public PlayerController PlayerController
	{
		get
		{
			return playerController;
		}
		set
		{
			playerController = value;
			AddRadarGuiElement(value);
		}
	}

	public static HUD Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (HUD)Object.FindObjectOfType(typeof(HUD));
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	public string ClipSize
	{
		get
		{
			return _clipSize;
		}
	}

	public string CurrentAmmo
	{
		get
		{
			return _currentAmmo;
		}
	}

	public float HealthPercentage
	{
		get
		{
			return _healthPercentage;
		}
	}

	public RadarGuiElement RadarElement
	{
		get
		{
			return _radarGuiElement;
		}
	}

	public bool RadarPurchased
	{
		get
		{
			return ServiceManager.Instance.IsItemBought(ServiceManager.Instance.GetItemByName("radar").id);
		}
	}

	public bool JumpPurchased
	{
		get
		{
			return ServiceManager.Instance.IsItemBought(ServiceManager.Instance.GetItemByName("jump").id);
		}
	}

	public bool JumpButtonEnabled
	{
		get
		{
			return _jumpButtonEnabled && JumpPurchased;
		}
	}

	public void SetRadarEnabled(bool state)
	{
		if (_radarGuiElement != null && playerController != null && PlayerController.PlayersGUI != null && RadarPurchased)
		{
			if (state)
			{
				playerController.PlayersGUI.AddComponent(_radarGuiElement);
			}
			else
			{
				playerController.PlayersGUI.RemoveComponent(_radarGuiElement);
			}
		}
	}

	private void AddRadarGuiElement(PlayerController controller)
	{
		if (Preferences.Instance.RadarToggledOn && RadarPurchased && Preferences.Instance.CurrentGameMode != GameMode.ROYL)
		{
			if (_radarGuiElement == null)
			{
				_radarGuiElement = base.gameObject.AddComponent<RadarGuiElement>();
			}
			_radarGuiElement.LocalPlayerController = playerController;
			SetRadarEnabled(Preferences.Instance.RadarToggledOn);
		}
	}

	public void ForceAddRadarForRoyale()
	{
		ForceAddRadar();
		_radarGuiElement.RenderRadius = 3000f;
	}

	public void ForceAddRadar()
	{
		if (_radarGuiElement == null)
		{
			_radarGuiElement = base.gameObject.AddComponent<RadarGuiElement>();
		}
		_radarGuiElement.LocalPlayerController = playerController;
		_radarGuiElement.PositionGUI();
		playerController.PlayersGUI.AddComponent(_radarGuiElement);
	}

	private void Awake()
	{
		UpdateLocalizationText();
		_radarGuiElement = GetComponent<RadarGuiElement>();
		_jumpButtonEnabled = true;
	}

	private void Start()
	{
		fpsText.gameObject.SetActive(false);
		if (pingText != null)
		{
			pingText.gameObject.SetActive(false);
		}
		blueTeamMessage.SetActive(false);
		redTeamMessage.SetActive(false);
		weaponCooldownbarObject = weaponCooldownbar.gameObject;
		secondaryReticle.SetActive(false);
		if (Application.loadedLevelName != "Tutorial")
		{
			if (Preferences.Instance.CurrentGameMode == GameMode.CTF)
			{
				arrowIndicator.gameObject.SetActive(true);
			}
			else
			{
				arrowIndicator.gameObject.SetActive(false);
			}
		}
		else
		{
			arrowIndicator.gameObject.SetActive(false);
		}
		redKills = "0";
		blueKills = "0";
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			InitializeRoyaleDisplay();
		}
	}

	private void InitializeRoyaleDisplay()
	{
		_FFAKills.gameObject.SetActive(false);
		_FFALeaderName.text = GameManager.Instance.playerStats.Count + " Remain";
		string text = "_red";
		PlayerLoadout currentLoadout = LoadoutManager.Instance.CurrentLoadout;
		Object @object = Resources.Load("Icons/Characters/" + currentLoadout.model.name + "/" + currentLoadout.skin.name + text);
		if (@object != null)
		{
			_currentFFALeaderIcon = Object.Instantiate(@object) as GameObject;
			_currentFFALeaderIcon.transform.parent = _iconMount;
			_currentFFALeaderIcon.transform.localPosition = Vector3.zero;
			_currentFFALeaderIcon.transform.localEulerAngles = Vector3.zero;
			_currentFFALeaderIcon.transform.localScale = new Vector3(1f, 1f, 1f);
			_currentFFALeaderIcon.layer = LayerMask.NameToLayer("HUD");
		}
		Vector3 position = _currentFFALeaderIcon.transform.position;
		Vector3 position2 = _FFAKills.transform.position;
		_currentFFALeaderIcon.transform.position = new Vector3(position2.x + 10f, position.y, position2.z);
	}

	private void UpdateLocalizationText()
	{
		_waitingForServer = Language.Get("GAME_STATUS_WAITING_FOR_SERVER");
	}

	public void OnShowStatsPage(bool isGameEnd)
	{
		if (currentStatsOverlay != null)
		{
			Object.Destroy(currentStatsOverlay);
		}
		if (currentPauseMenu != null)
		{
			Object.Destroy(currentPauseMenu);
		}
		if (currentTeamspeakOverlay != null)
		{
			Object.Destroy(currentTeamspeakOverlay);
		}
		if (Preferences.Instance.CurrentGameMode != GameMode.ROYL)
		{
			GameObject original = ((Preferences.Instance.CurrentGameMode != 0) ? teamStatsOverlay : ffaStatsOverlay);
			currentStatsOverlay = Object.Instantiate(original) as GameObject;
			currentStatsOverlay.transform.parent = base.transform;
			currentStatsOverlay.transform.localPosition = new Vector3(-2f, 2.5f, -350f);
			currentStatsOverlay.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			StatsOverlay component = currentStatsOverlay.GetComponent<StatsOverlay>();
			if (!isGameEnd && lastKiller != null)
			{
				component.OnUpdateKillerInfo(lastKiller);
			}
			if (isGameEnd)
			{
				component.tapText.text = _waitingForServer;
				component.toggleLoadoutButton.SetActive(false);
			}
			else
			{
				component.startRespawnCountdown();
			}
		}
	}

	public void OnPlayTutorialAnimation(bool isFromTutorial)
	{
		if (lobbyCamera != null)
		{
			Object.Destroy(lobbyCamera);
		}
		PlayerPrefs.SetString("played_once", string.Empty);
	}

	public void OnAddHealthNotification(string text)
	{
		addNotification(healthNotification, text);
	}

	public void OnAddChickenNotification(string text)
	{
		addNotification(chickenNotification, text);
	}

	public void OnAddPowerupNotification()
	{
		addNotification(powerupNotification, string.Empty);
	}

	public void OnAddShieldsNotification()
	{
		addNotification(shieldNotification, string.Empty);
	}

	public void OnAddJoulesNotification(string text)
	{
		addNotification(joulesNotification, text);
	}

	public void OnAddRewardNotification(string joules, string xp)
	{
		GameObject gameObject = addNotification(rewardNotification, string.Empty);
		TextMesh component = gameObject.transform.Find("joules").GetComponent<TextMesh>();
		TextMesh component2 = gameObject.transform.Find("xp").GetComponent<TextMesh>();
		component.text = joules;
		component2.text = xp;
	}

	public void OnAddKillStreakNotification(string streak, string jouleReward)
	{
		GameObject gameObject = addNotification(killStreakNotification, string.Empty);
		TextMesh component = gameObject.transform.Find("joules").GetComponent<TextMesh>();
		TextMesh component2 = gameObject.transform.Find("streak").GetComponent<TextMesh>();
		component.text = jouleReward;
		component2.text = streak;
	}

	public void OnSuicideNotification(string characterModel, string characterSkin, Team characterTeam)
	{
		GameObject gameObject = addNotification(suicideNotification, "suicide");
		Transform parent = gameObject.transform.Find("character_icon");
		Object @object = Resources.Load("Icons/Characters/" + characterModel + "/" + characterSkin + ((characterTeam != 0) ? "_blue" : "_red"));
		if (@object != null)
		{
			GameObject gameObject2 = Object.Instantiate(@object) as GameObject;
			gameObject2.transform.parent = parent;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = LayerMask.NameToLayer("HUD");
		}
	}

	public void OnAddBombHoldBonusNotification(Team characterTeam, string jouleReward)
	{
		addBombNotification(bombHoldNotification, characterTeam, jouleReward);
	}

	public void OnAddBombDepositNotification(Team characterTeam, string jouleReward)
	{
		addBombNotification(bombDepositNotification, characterTeam, jouleReward);
	}

	private void addBombNotification(GameObject notification, Team characterTeam, string jouleReward)
	{
		GameObject gameObject = addNotification(notification, string.Empty);
		TextMesh component = gameObject.transform.Find("joules").GetComponent<TextMesh>();
		if (component != null)
		{
			component.text = jouleReward;
		}
		Transform parent = gameObject.transform.Find("bomb_icon");
		Object @object = Resources.Load("CTF/Bombs/bombIcon" + ((characterTeam != 0) ? "_blue" : "_red"));
		if (@object != null)
		{
			GameObject gameObject2 = Object.Instantiate(@object) as GameObject;
			gameObject2.transform.parent = parent;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = LayerMask.NameToLayer("HUD");
		}
	}

	public void OnAddStopStreakNotification(string jouleReward)
	{
		addNotification(stopStreakNotification, jouleReward);
	}

	public void OnAddAssistNotication(string jouleReward)
	{
		addNotification(assistNotification, jouleReward);
	}

	public void OnAddTimeNotification()
	{
		GameObject gameObject = Object.Instantiate(mainTimeNotification) as GameObject;
		gameObject.transform.parent = mainTimerMount;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void OnPlaySound(AudioClip clip)
	{
		AudioSource.PlayClipAtPoint(clip, Vector3.zero);
	}

	public void OnSpeakNotification(string speakerModel, string speakerSkin, Team speakerTeam, string speakIcon)
	{
		GameObject gameObject = addNotification(speakNotification, string.Empty);
		Transform parent = gameObject.transform.Find("character_icon");
		Transform parent2 = gameObject.transform.Find("teamSpeak_icon");
		Object @object = Resources.Load("Icons/Characters/" + speakerModel + "/" + speakerSkin + ((speakerTeam != 0) ? "_blue" : "_red"));
		if (@object != null)
		{
			GameObject gameObject2 = Object.Instantiate(@object) as GameObject;
			gameObject2.transform.parent = parent;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = LayerMask.NameToLayer("HUD");
		}
		@object = Resources.Load("Icons/TeamSpeak/" + speakIcon);
		if (@object != null)
		{
			GameObject gameObject3 = Object.Instantiate(@object) as GameObject;
			gameObject3.transform.parent = parent2;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localEulerAngles = Vector3.zero;
			gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject3.layer = LayerMask.NameToLayer("HUD");
		}
	}

	public void OnKillNotification(string killerModel, string killerSkin, Team killerTeam, string victimModel, string victimSkin, Team victimTeam, bool isHeadshot)
	{
		if (currentStatsOverlay != null)
		{
			currentStatsOverlay.SendMessage("OnUpdateStats");
		}
		GameObject gameObject = ((!isHeadshot) ? addNotification(killNotification, string.Empty) : addNotification(headshotNotification, string.Empty));
		Transform parent = gameObject.transform.Find("killer_icon");
		Transform parent2 = gameObject.transform.Find("victim_icon");
		Object @object = Resources.Load("Icons/Characters/" + killerModel + "/" + killerSkin + ((killerTeam != 0) ? "_blue" : "_red"));
		if (@object != null)
		{
			GameObject gameObject2 = Object.Instantiate(@object) as GameObject;
			gameObject2.transform.parent = parent;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localEulerAngles = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = LayerMask.NameToLayer("HUD");
		}
		@object = Resources.Load("Icons/Characters/" + victimModel + "/" + victimSkin + ((victimTeam != 0) ? "_blue" : "_red"));
		if (@object != null)
		{
			GameObject gameObject3 = Object.Instantiate(@object) as GameObject;
			gameObject3.transform.parent = parent2;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localEulerAngles = Vector3.zero;
			gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject3.layer = LayerMask.NameToLayer("HUD");
		}
	}

	public GameObject addNotification(GameObject notificationPrefab, string text)
	{
		foreach (Transform item in notificationsRoot)
		{
			Vector3 localPosition = item.localPosition;
			localPosition.y -= notificationPushAmount;
			item.localPosition = localPosition;
		}
		GameObject gameObject = Object.Instantiate(notificationPrefab) as GameObject;
		gameObject.transform.parent = notificationsRoot;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		Transform transform2 = gameObject.transform.Find("text");
		if (transform2 != null)
		{
			(transform2.GetComponent(typeof(TextMesh)) as TextMesh).text = text;
		}
		return gameObject;
	}

	public void OnSetReloadDisplay(float reloadTime)
	{
		nextReloadEndTime = Time.time + reloadTime;
		reloadDuration = reloadTime;
	}

	public bool ReloadInProgress()
	{
		return Time.time < nextReloadEndTime;
	}

	public void OnSetFPS(float fps)
	{
		fpsText.text = string.Format("FPS: {0:#.##}", fps);
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		hasPressedButton = true;
		switch (b.name)
		{
		case "exitTutorial_btn":
			Application.LoadLevel("MainMenu");
			break;
		case "bombTutorial":
			Object.Destroy(b.gameObject);
			break;
		case "teamspeakButton":
			createTeamSpeakPopup();
			break;
		case "specialButton":
			if (isSpecialAllowed && playerController != null && !playerController.HasBomb && playerController.NextSpecialItemChargeTime - Time.time <= 0f)
			{
				if (playerController.CharacterManager != null && playerController.CharacterManager.playerLoadout.special != null && playerController.CharacterManager.playerLoadout.special.name == "SlowMo")
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["PAWSED"]);
				}
				currentSpecialItemCooldownTime = playerController.OnUseSpecialItem();
			}
			break;
		case "pauseButton":
			createPauseMenu();
			break;
		}
		if (!b.name.StartsWith("speak_"))
		{
			return;
		}
		string[] array = b.name.Split('_');
		if (playerController != null)
		{
			switch (array[1])
			{
			case "followMe":
				playerController.OnTeamSpeak(0);
				break;
			case "help":
				playerController.OnTeamSpeak(1);
				break;
			case "attack":
				playerController.OnTeamSpeak(2);
				break;
			case "incoming":
				playerController.OnTeamSpeak(3);
				break;
			case "wooHoo":
				CumulativeStats.Instance.numberOfWoohoos++;
				if (CumulativeStats.Instance.numberOfWoohoos <= 25)
				{
					Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["AWW_YEAH"], (float)CumulativeStats.Instance.numberOfWoohoos / 25f * 100f);
				}
				playerController.OnTeamSpeak(4);
				break;
			}
		}
		dismissTeamSpeakPopup();
	}

	public void createTeamSpeakPopup()
	{
		if (currentTeamspeakOverlay != null)
		{
			Object.Destroy(currentTeamspeakOverlay);
			return;
		}
		currentTeamspeakOverlay = Object.Instantiate(teamspeakOverlayPrefab) as GameObject;
		currentTeamspeakOverlay.transform.parent = base.transform;
		currentTeamspeakOverlay.transform.localPosition = new Vector3(-50f, 0f, -100f);
		currentTeamspeakOverlay.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
	}

	private void dismissTeamSpeakPopup()
	{
		CumulativeStats.Instance.numberOfTeamspeaks++;
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["BEARER_OF_BAD_NEWS"]);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["DO_I_TALK_A_LOT"], (float)CumulativeStats.Instance.numberOfTeamspeaks / 100f * 100f);
		Object.Destroy(currentTeamspeakOverlay);
	}

	public void createPauseMenu()
	{
		if (currentTeamspeakOverlay != null)
		{
			Object.Destroy(currentTeamspeakOverlay);
		}
		if (currentPauseMenu != null)
		{
			Object.Destroy(currentPauseMenu);
			return;
		}
		currentPauseMenu = Object.Instantiate(pauseMenu) as GameObject;
		currentPauseMenu.transform.parent = base.transform;
		currentPauseMenu.transform.localPosition = new Vector3(0f, 9f, -275f);
		currentPauseMenu.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
	}

	public void OnGameEnd(Team winningTeam, List<PlayerLoadout> winningLoadouts)
	{
		guiController.IsActive = false;
		StartCoroutine(handleGameEnd(winningTeam, winningLoadouts));
	}

	private IEnumerator handleGameEnd(Team winningTeam, List<PlayerLoadout> winningLoadouts)
	{
		if (currentStatsOverlay != null)
		{
			Object.Destroy(currentStatsOverlay);
		}
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			HandleRoyaleGameEnd(winningLoadouts);
			yield break;
		}
		GameObject newEndPopup = Object.Instantiate(endGamePopup) as GameObject;
		newEndPopup.transform.parent = base.transform;
		newEndPopup.transform.localPosition = new Vector3(0f, 0f, -450f);
		newEndPopup.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		switch (winningTeam)
		{
		case Team.RED:
			newEndPopup.transform.Find("blue_wins").gameObject.SetActive(false);
			break;
		case Team.BLUE:
			newEndPopup.transform.Find("red_wins").gameObject.SetActive(false);
			break;
		}
		for (int i = 0; i < winningLoadouts.Count; i++)
		{
			Transform currentMount = newEndPopup.transform.Find("head_mount" + i);
			if (currentMount != null)
			{
				Object icon = null;
				switch (winningTeam)
				{
				case Team.RED:
					icon = Resources.Load("Icons/Characters/" + winningLoadouts[i].model.name + "/" + winningLoadouts[i].skin.name + "_red");
					break;
				case Team.BLUE:
					icon = Resources.Load("Icons/Characters/" + winningLoadouts[i].model.name + "/" + winningLoadouts[i].skin.name + "_blue");
					break;
				}
				if (icon != null)
				{
					GameObject newIcon = Object.Instantiate(icon) as GameObject;
					newIcon.transform.parent = currentMount;
					newIcon.transform.localPosition = Vector3.zero;
					newIcon.transform.localEulerAngles = Vector3.zero;
					newIcon.transform.localScale = new Vector3(1f, 1f, 1f);
					newIcon.layer = LayerMask.NameToLayer("HUD");
				}
			}
		}
		yield return new WaitForSeconds(2.5f);
		Object.Destroy(newEndPopup);
		OnShowStatsPage(true);
	}

	private void HandleRoyaleGameEnd(List<PlayerLoadout> winningLoadouts)
	{
		GameObject gameObject = Object.Instantiate(_royaleEndGamePopup) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -450f);
		gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		gameObject.GetComponent<RoyaleGameEndPopup>().Init(winningLoadouts);
	}

	private void Update()
	{
		if (playerController == null)
		{
			return;
		}
		if (currentTeamspeakOverlay != null && playerController.IsDead)
		{
			dismissTeamSpeakPopup();
		}
		MogaController mogaController = MogaController.Instance;
		if (mogaController.connection == 1)
		{
			bool flag = Instance.currentTeamspeakOverlay != null;
			if (!flag)
			{
				if (mogaController.ButtonPressed(108) && Instance.currentPauseMenu == null && Instance.currentStatsOverlay == null)
				{
					createPauseMenu();
				}
				if (mogaController.ButtonPressed(109) && Instance.currentPauseMenu == null && Instance.currentStatsOverlay == null)
				{
					createTeamSpeakPopup();
				}
			}
			if (flag)
			{
				if (mogaController.ButtonPressed(103) || mogaController.ButtonPressed(105))
				{
					playerController.OnTeamSpeak(0);
					dismissTeamSpeakPopup();
				}
				if (mogaController.ButtonPressed(102) || mogaController.ButtonPressed(104))
				{
					playerController.OnTeamSpeak(1);
					dismissTeamSpeakPopup();
				}
				if (mogaController.ButtonPressed(96))
				{
					playerController.OnTeamSpeak(2);
					dismissTeamSpeakPopup();
				}
				if (mogaController.ButtonPressed(99))
				{
					playerController.OnTeamSpeak(3);
					dismissTeamSpeakPopup();
				}
				if (mogaController.ButtonPressed(100))
				{
					playerController.OnTeamSpeak(4);
					CumulativeStats.Instance.numberOfWoohoos++;
					dismissTeamSpeakPopup();
				}
			}
		}
		if (arrowIndicator.gameObject.activeInHierarchy && CTFManager.Instance != null && !GameManager.Instance.IsGameSubmitted)
		{
			Transform transform;
			if (playerController.Team == Team.RED)
			{
				if (PlayerController.HasBomb)
				{
					bombTextMesh.text = CTFManager.Instance.getFormattedTime(CTFManager.Instance.RedTimeLeft);
					transform = CTFManager.Instance.BlueDepositSpot;
				}
				else
				{
					GameObject redBomb = CTFManager.Instance.RedBomb;
					transform = ((!(redBomb != null)) ? CTFManager.Instance.BlueDepositSpot : redBomb.transform);
				}
			}
			else if (PlayerController.HasBomb)
			{
				bombTextMesh.text = CTFManager.Instance.getFormattedTime(CTFManager.Instance.BlueTimeLeft);
				transform = CTFManager.Instance.RedDepositSpot;
			}
			else
			{
				GameObject blueBomb = CTFManager.Instance.BlueBomb;
				transform = ((!(blueBomb != null)) ? CTFManager.Instance.RedDepositSpot : blueBomb.transform);
			}
			Transform transform2 = playerController.transform;
			Vector3 vector = transform.position - transform2.position;
			Vector3 forward = transform2.forward;
			Vector2 from = new Vector2(vector.x, vector.z);
			Vector2 to = new Vector2(forward.x, forward.z);
			float num = Vector2.Angle(from, to);
			float z = Vector3.Cross(new Vector3(from.x, from.y, 0f), new Vector3(to.x, to.y, 0f)).z;
			float num2 = 0f;
			if (vector.magnitude > 1f)
			{
				num2 = Mathf.Asin(vector.y / vector.magnitude) * 57.29578f;
			}
			Vector3 localEulerAngles = arrowIndicator.transform.localEulerAngles;
			if (z < 0f)
			{
				localEulerAngles.y = 360f - num;
			}
			else
			{
				localEulerAngles.y = num;
			}
			float b = num2;
			if (num2 >= 0f && num2 < 10f)
			{
				b = 10f;
			}
			else if (num2 < 0f && num2 > -10f)
			{
				b = -10f;
			}
			localEulerAngles.x = Mathf.LerpAngle(localEulerAngles.x, b, 5f * Time.deltaTime);
			arrowIndicator.transform.localEulerAngles = localEulerAngles;
		}
		if (Preferences.Instance.CurrentGameMode == GameMode.KOTH && KOTHManager.Instance.CurrentPoint != null)
		{
			arrowIndicator.gameObject.SetActive(true);
			Transform transform3 = KOTHManager.Instance.CurrentPoint.tracker.transform;
			Transform transform4 = playerController.transform;
			Vector3 vector2 = transform3.position - transform4.position;
			Vector3 forward2 = transform4.forward;
			Vector2 from2 = new Vector2(vector2.x, vector2.z);
			Vector2 to2 = new Vector2(forward2.x, forward2.z);
			float num3 = Vector2.Angle(from2, to2);
			float z2 = Vector3.Cross(new Vector3(from2.x, from2.y, 0f), new Vector3(to2.x, to2.y, 0f)).z;
			float num4 = 0f;
			if (vector2.magnitude > 1f)
			{
				num4 = Mathf.Asin(vector2.y / vector2.magnitude) * 57.29578f;
			}
			Vector3 localEulerAngles2 = arrowIndicator.transform.localEulerAngles;
			if (z2 < 0f)
			{
				localEulerAngles2.y = 360f - num3;
			}
			else
			{
				localEulerAngles2.y = num3;
			}
			float b2 = num4;
			if (num4 >= 0f && num4 < 10f)
			{
				b2 = 10f;
			}
			else if (num4 < 0f && num4 > -10f)
			{
				b2 = -10f;
			}
			localEulerAngles2.x = Mathf.LerpAngle(localEulerAngles2.x, b2, 5f * Time.deltaTime);
			arrowIndicator.transform.localEulerAngles = localEulerAngles2;
		}
		float num5 = playerController.NextSpecialItemChargeTime - Time.time;
		num5 = nextReloadEndTime - Time.time;
		if (num5 > 0f)
		{
			if (!weaponCooldownbarObject.activeInHierarchy)
			{
				weaponCooldownbarObject.SetActive(true);
			}
			if (isHuggable)
			{
				if (!abilityChargingOverlay.activeInHierarchy)
				{
					abilityChargingOverlay.SetActive(true);
				}
			}
			else if (!weaponReloadOverlay.activeInHierarchy)
			{
				weaponReloadOverlay.SetActive(true);
			}
			Vector3 localScale = weaponCooldownbar.localScale;
			localScale.y = num5 / reloadDuration;
			weaponCooldownbar.localScale = localScale;
		}
		else
		{
			if (weaponCooldownbarObject.activeInHierarchy)
			{
				weaponCooldownbarObject.SetActive(false);
			}
			if (isHuggable)
			{
				if (abilityChargingOverlay.activeInHierarchy)
				{
					abilityChargingOverlay.SetActive(false);
				}
			}
			else if (weaponReloadOverlay.activeInHierarchy)
			{
				weaponReloadOverlay.SetActive(false);
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape) && playerController != null && !playerController.IsDead)
		{
			createPauseMenu();
			CustomLayoutController.Instance.Close();
		}
		if (pingText != null)
		{
			pingText.text = "PING: " + PhotonNetwork.GetPing();
		}
		if (_previousLeader == null && Preferences.Instance.CurrentGameMode == GameMode.FFA && !Bootloader.Instance.InTutorial)
		{
			UpdateLeaderDisplay();
		}
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			UpdateRoyaleDisplay(GameManager.Instance.RoyaleRemainingPlayers());
		}
	}

	public void UpdateLeaderDisplay()
	{
		PlayerStats fFALeader = GameManager.Instance.GetFFALeader();
		_FFALeaderName.text = fFALeader.greeName;
		_FFAKills.text = fFALeader.NetKills.ToString();
		if (_previousLeader != null && _previousLeader == fFALeader)
		{
			return;
		}
		if (_currentFFALeaderIcon != null)
		{
			Object.Destroy(_currentFFALeaderIcon);
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(fFALeader.id);
		if (playerCharacterManager != null && playerCharacterManager.PlayerController != null)
		{
			string text = ((playerCharacterManager.PlayerController.Team != 0) ? "_blue" : "_red");
			Object @object = Resources.Load("Icons/Characters/" + playerCharacterManager.playerLoadout.model.name + "/" + playerCharacterManager.playerLoadout.skin.name + text);
			if (@object != null)
			{
				_currentFFALeaderIcon = Object.Instantiate(@object) as GameObject;
				_currentFFALeaderIcon.transform.parent = _iconMount;
				_currentFFALeaderIcon.transform.localPosition = Vector3.zero;
				_currentFFALeaderIcon.transform.localEulerAngles = Vector3.zero;
				_currentFFALeaderIcon.transform.localScale = new Vector3(1f, 1f, 1f);
				_currentFFALeaderIcon.layer = LayerMask.NameToLayer("HUD");
			}
		}
	}

	public void UpdateRoyaleDisplay(int remainingPlayers)
	{
		_FFALeaderName.text = remainingPlayers + " Remain";
	}

	public void SwitchedWeapons()
	{
		if (isUsingPrimary)
		{
			weaponSwitchAnimator.Play("pri_alt");
		}
		else
		{
			weaponSwitchAnimator.Play("alt_pri");
		}
		isUsingPrimary = !isUsingPrimary;
		primaryReticle.SetActive(isUsingPrimary);
		secondaryReticle.SetActive(!isUsingPrimary);
	}

	public void OnShowControlMode(ControlMode hudMode)
	{
		if (!(PlayerController.Director is WASDControllerDirector) && PlayerController != null)
		{
			if (PlayerController.Director != null)
			{
				((PlayerControllerDirector)PlayerController.Director).RemoveFrom(playerController);
				Object.Destroy(playerController.Director);
			}
			if (PlayerController.SatelliteSecondaries)
			{
				if (Preferences.Instance.CurrentShootMode == ShootMode.dualJoystick)
				{
					PlayerController.Director = PlayerController.gameObject.AddComponent<DualJoystickControllerDirector>();
				}
				else if (Preferences.Instance.CurrentShootMode == ShootMode.shootButton)
				{
					PlayerController.Director = PlayerController.gameObject.AddComponent<SatelliteSecondaryShootButtonDirector>();
				}
				else if (Preferences.Instance.CurrentShootMode == ShootMode.keyboardAndMouse)
				{
					PlayerController.Director = PlayerController.gameObject.AddComponent<SateliteKeyboardAndMouseControllerDirector>();
				}
				else
				{
					PlayerController.Director = PlayerController.gameObject.AddComponent<SatelliteSecondaryDoubleTapDirector>();
				}
			}
			else if (Preferences.Instance.CurrentShootMode == ShootMode.dualJoystick)
			{
				PlayerController.Director = PlayerController.gameObject.AddComponent<DualJoystickControllerDirector>();
			}
			else if (Preferences.Instance.CurrentShootMode == ShootMode.shootButton)
			{
				PlayerController.Director = PlayerController.gameObject.AddComponent<ShootButtonControllerDirector>();
			}
			else if (Preferences.Instance.CurrentShootMode == ShootMode.keyboardAndMouse)
			{
				PlayerController.Director = PlayerController.gameObject.AddComponent<KeyboardAndMouseControllerDirector>();
			}
			else
			{
				PlayerController.Director = PlayerController.gameObject.AddComponent<DoubleTapSwipeAreaControllerDirector>();
			}
			if (MogaController.Instance.connection == 1)
			{
				if (playerController.SatelliteSecondaries)
				{
					PlayerController.Director = PlayerController.gameObject.AddComponent<SatelliteMogaControllerDirector>();
				}
				else
				{
					PlayerController.Director = PlayerController.gameObject.AddComponent<MogaControllerDirector>();
				}
			}
			((PlayerControllerDirector)PlayerController.Director).AddTo(PlayerController);
		}
		SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)PlayerController.Director;
		if (PlayerController.specialItemPrefab != null)
		{
			SpecialItem component = PlayerController.specialItemPrefab.GetComponent<SpecialItem>();
			if (component != null && simpleControllerDirector != null)
			{
				simpleControllerDirector.SpecialButton.Icon = Resources.Load(component.IconTextureLocation) as Texture2D;
			}
		}
		else if (simpleControllerDirector != null)
		{
			simpleControllerDirector.SpecialButton.RemoveFrom(playerController.PlayersGUI);
		}
		if (hudMode == ControlMode.custom)
		{
			GUIPositionController.Instance.LoadFromPlayerPrefs();
		}
		else
		{
			GUIPositionController.Instance.SetToDefaultPositions();
		}
		if (simpleControllerDirector != null)
		{
			simpleControllerDirector.PositionGUI();
			if (_radarGuiElement != null)
			{
				_radarGuiElement.PositionGUI();
			}
		}
	}

	public void ReloadSpecialIcon()
	{
		SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)PlayerController.Director;
		if (PlayerController.specialItemPrefab != null)
		{
			SpecialItem component = PlayerController.specialItemPrefab.GetComponent<SpecialItem>();
			if (component != null && simpleControllerDirector != null)
			{
				simpleControllerDirector.SpecialButton.Icon = Resources.Load(component.IconTextureLocation) as Texture2D;
				simpleControllerDirector.SpecialButton.AddTo(playerController.PlayersGUI);
			}
		}
		else if (simpleControllerDirector != null)
		{
			simpleControllerDirector.SpecialButton.RemoveFrom(playerController.PlayersGUI);
		}
		if (simpleControllerDirector != null)
		{
			simpleControllerDirector.PositionGUI();
			if (_radarGuiElement != null)
			{
				_radarGuiElement.PositionGUI();
			}
		}
	}

	public void OnGUIButtonPressed(GUIButton b)
	{
		hasPressedButton = true;
		switch (b.name)
		{
		case "meleeButton":
			if (playerController != null && !playerController.HasBomb)
			{
				playerController.WeaponManager.OnMeleeAttack();
			}
			break;
		}
	}

	public void OnSetClipSize(float amount)
	{
		_clipSize = ((int)amount).ToString();
	}

	public void OnSetAmmo(float amount)
	{
		_currentAmmo = ((int)amount).ToString();
	}

	public void OnReset()
	{
		if (!isUsingPrimary)
		{
			weaponSwitchAnimator.Play("alt_pri");
		}
		isUsingPrimary = true;
		primaryReticle.SetActive(isUsingPrimary);
		secondaryReticle.SetActive(!isUsingPrimary);
	}

	public void OnSetHealth(float healthPercentage)
	{
		if (healthPercentage > 1f)
		{
			healthPercentage = 1f;
		}
		if (healthPercentage < 0f)
		{
			healthPercentage = 0f;
		}
		_healthPercentage = healthPercentage;
		if (playerController != null)
		{
			SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)playerController.Director;
			if (simpleControllerDirector != null)
			{
				simpleControllerDirector.HealthBar.DesiredSlidePercent = healthPercentage;
			}
		}
	}

	public void DestroyCurrentIcons()
	{
		foreach (GameObject currentIcon in currentIcons)
		{
			Object.Destroy(currentIcon);
		}
		currentIcons.Clear();
	}

	public void OnSetTeam(Team t)
	{
		DestroyCurrentIcons();
		string empty = string.Empty;
		empty = ((t != 0) ? "_blue" : "_red");
		if (Application.loadedLevelName != "Tutorial" && Application.loadedLevelName != "WeaponTest")
		{
			Object @object = Resources.Load("Icons/Characters/" + LoadoutManager.Instance.CurrentLoadout.model.name + "/" + LoadoutManager.Instance.CurrentLoadout.skin.name + empty);
			if (@object != null)
			{
				GameObject gameObject = Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = iconMount;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.layer = LayerMask.NameToLayer("HUD");
				currentIcons.Add(gameObject);
			}
			Object object2 = Resources.Load("Icons/Weapons/" + LoadoutManager.Instance.CurrentLoadout.model.name + "/" + LoadoutManager.Instance.CurrentLoadout.primary.name);
			if (object2 != null)
			{
				GameObject gameObject2 = Object.Instantiate(object2) as GameObject;
				gameObject2.transform.parent = primaryMount;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localEulerAngles = Vector3.zero;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject2.layer = LayerMask.NameToLayer("HUD");
				currentIcons.Add(gameObject2);
			}
			Object object3 = Resources.Load("Icons/Weapons/" + LoadoutManager.Instance.CurrentLoadout.model.name + "/" + LoadoutManager.Instance.CurrentLoadout.secondary.name);
			if (object3 != null)
			{
				GameObject gameObject3 = Object.Instantiate(object3) as GameObject;
				gameObject3.transform.parent = secondaryMount;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.transform.localEulerAngles = Vector3.zero;
				gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject3.layer = LayerMask.NameToLayer("HUD");
				currentIcons.Add(gameObject3);
			}
			if (Preferences.Instance != null && Preferences.Instance.IsTeamMode)
			{
				if (t == Team.RED)
				{
					redTeamMessage.SetActive(true);
				}
				else
				{
					blueTeamMessage.SetActive(true);
				}
				StartCoroutine(disableTeamMessage());
			}
		}
		else
		{
			Object object4 = Resources.Load("Icons/Characters/Oliver/brown_red");
			if (object4 != null)
			{
				GameObject gameObject4 = Object.Instantiate(object4) as GameObject;
				gameObject4.transform.parent = iconMount;
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localEulerAngles = Vector3.zero;
				gameObject4.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject4.layer = LayerMask.NameToLayer("HUD");
			}
			Object object5 = Resources.Load("Icons/Weapons/Oliver/AR");
			if (object5 != null)
			{
				GameObject gameObject5 = Object.Instantiate(object5) as GameObject;
				gameObject5.transform.parent = primaryMount;
				gameObject5.transform.localPosition = Vector3.zero;
				gameObject5.transform.localEulerAngles = Vector3.zero;
				gameObject5.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject5.layer = LayerMask.NameToLayer("HUD");
			}
			Object object6 = Resources.Load("Icons/Weapons/Oliver/rocket_bearzooka");
			if (object6 != null)
			{
				GameObject gameObject6 = Object.Instantiate(object6) as GameObject;
				gameObject6.transform.parent = secondaryMount;
				gameObject6.transform.localPosition = Vector3.zero;
				gameObject6.transform.localEulerAngles = Vector3.zero;
				gameObject6.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject6.layer = LayerMask.NameToLayer("HUD");
			}
			Object object7 = Resources.Load("Icons/Weapons/Oliver/Standard_krikket");
			if (object7 != null)
			{
				GameObject gameObject7 = Object.Instantiate(object7) as GameObject;
				gameObject7.transform.parent = meleeMount;
				gameObject7.transform.localPosition = Vector3.zero;
				gameObject7.transform.localEulerAngles = Vector3.zero;
				gameObject7.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject7.layer = LayerMask.NameToLayer("HUD");
			}
			Object object8 = Resources.Load("Icons/Specials/JumpBoots");
			if (object8 != null)
			{
				GameObject gameObject8 = Object.Instantiate(object8) as GameObject;
				gameObject8.transform.parent = specialMount;
				gameObject8.transform.localPosition = Vector3.zero;
				gameObject8.transform.localEulerAngles = Vector3.zero;
				gameObject8.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject8.layer = LayerMask.NameToLayer("HUD");
			}
		}
		weaponReloadOverlay.SetActive(false);
		abilityChargingOverlay.SetActive(false);
		OnToggleBomb(false);
	}

	public void OnToggleBomb(bool toggle)
	{
		if (toggle)
		{
			if (Bootloader.Instance.isIpad)
			{
				base.animation["bombIn_ipad"].layer = 1;
				base.animation.Play("bombIn_ipad");
			}
			else
			{
				base.animation["bombIn"].layer = 1;
				base.animation.Play("bombIn");
			}
			StartCoroutine("arrowAnimator");
		}
		else
		{
			base.animation["bombOut"].layer = 1;
			base.animation.Play("bombOut");
			StopCoroutine("arrowAnimator");
		}
		if (playerController != null && playerController.Director is SimpleControllerDirector)
		{
			SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)playerController.Director;
			simpleControllerDirector.ToggleSwapArea(!toggle);
			simpleControllerDirector.ToggleAmmoArea(!toggle);
		}
	}

	private IEnumerator arrowAnimator()
	{
		yield return new WaitForSeconds(2f);
		float minWaitTime = arrowIndicator.animation["arrow"].length;
		while (true)
		{
			Transform targetTransform = ((playerController.Team != 0) ? CTFManager.Instance.RedDepositSpot : CTFManager.Instance.BlueDepositSpot);
			float distance = Vector3.Distance(b: playerController.transform.position, a: targetTransform.position);
			arrowIndicator.animation["arrow"].layer = 0;
			arrowIndicator.animation.Play("arrow");
			float pulseDelay = minWaitTime + distance / 1500f;
			yield return new WaitForSeconds(pulseDelay);
		}
	}

	private IEnumerator disableTeamMessage()
	{
		yield return new WaitForSeconds(6f);
		blueTeamMessage.SetActive(false);
		redTeamMessage.SetActive(false);
	}

	public void OnSetScore(Team team, int score)
	{
		if (team == Team.RED)
		{
			redKills = score.ToString();
		}
		else
		{
			blueKills = score.ToString();
		}
	}
}
