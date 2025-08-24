using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using LitJson;
using Prime31;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	private class StockPrice
	{
		public string change { get; set; }
	}

	public enum Menu
	{
		Main = 0,
		GearUp = 1,
		BundleDeals = 2,
		BuyBundle = 3,
		MatchSelect = 4,
		Customize = 5,
		Matchmaking = 6
	}

	private const string _flurryEventExclusionText = "select#";

	private const string DAY_THREE_PUSH_MESSAGE = "Hey, have you shot any bears lately? Come and shoot some bears!";

	private const string DAY_FOUR_PUSH_MESSAGE = "Hey, there are some unshot huggables around, come and shoot them!";

	private const string DAY_SIX_PUSH_MESSAGE = "Come shoot some bees at people!";

	private const string DAY_THREE_ACTION = "Bear Power!";

	private const string DAY_FOUR_ACTION = "Hugs!";

	private const string DAY_SIX_ACTION = "Bzzz!";

	private const string _w3iBundleID = "net.skyvu.battlebearsgold";

	private const int _w3iAppID = 13386;

	private const string _w3iAppName = "Battle Bears Gold";

	private static MainMenu _instance;

	public static bool isFirstTime = true;

	public static bool wasDisconnected;

	public static bool gameCancelled;

	public static bool hasInvalidSession;

	public static bool hasBeenToMainMenuScene;

	[SerializeField]
	private GameObject _reputationPopup;

	[SerializeField]
	private GameObject _reputationRewardPopup;

	[SerializeField]
	private GameObject _maliciousPlayerPopup;

	public GUIController guiController;

	public string currentMenuAnimation = "start_main";

	private Animation myAnimation;

	public ButtonColliderManager menuColliders;

	public MenuAreaManager menuAreas;

	public Transform characterMountpoint;

	public GameObject currentCharacter;

	private GameObject currentRighthandWeapon;

	private GameObject currentLefthandWeapon;

	private GameObject currentBackpackWeapon;

	private bool _characterUsesBackpack;

	private bool _attachMeleeToBackpack;

	private Animation characterAnimator;

	private Transform leftMountpoint;

	private Transform rightMountpoint;

	private Transform backMountpoint;

	private bool isMatchmaking;

	private string matchmakingServerURL;

	public TextMesh[] levelTexts;

	public Transform[] levelChargebars;

	public Transform textRoot;

	public Transform[] headIconMounts;

	private GameObject[] headIcons;

	public TextMesh[] currentJoulesVisuals;

	public TextMesh[] currentGasVisuals;

	private int matchChoiceIndex;

	public TextMesh averageWaitTimeText;

	public TextMesh userNumberText;

	public TextMesh timeElapsed;

	public TextMesh messageOfTheDay;

	private bool isLoadingLevel;

	public ImageAnimator matchmakingImageAnimator;

	public Transform[] energyBars;

	public TextMesh[] refillTexts;

	public TextMesh[] rankTexts;

	public Transform[] rankIconMounts;

	public Transform largeMedalRoot;

	public Transform medalRankRoot;

	public Transform popupRoot;

	public GameObject optionsPopup;

	public GameObject extrasPopup;

	public GameObject buyGasPopup;

	public GameObject buyJoulesPopup;

	public GameObject outOfEnergy;

	public GameObject tradeGasShopPopup;

	public GameObject levelupPopup;

	public GameObject rankupPopup;

	public GameObject dailyRewardPopup;

	public GameObject tutorialPopup;

	public GameObject guestGasWarningPopup;

	public GameObject buyGoldPackPopup;

	public GameObject privateMatchPopup;

	public GameObject windows8ControlPopup;

	public GameObject errorMessage;

	public GameObject currentPopup;

	private Queue<SetupPopup> popUpQueue = new Queue<SetupPopup>();

	public AnimatedScroller gameChooseScroller;

	public string[] motds;

	private double preFillGasLevel;

	public AudioClip gameStartSound;

	public AudioClip[] clickSounds;

	public AudioClip rotateLeftSound;

	public AudioClip rotateRightSound;

	public SpecialOffersManager specialOffersManager;

	public GameObject fader;

	public GameObject createAccountButton;

	private int numTimesPlayed;

	public Camera popupCamera;

	public GameObject accessDenied;

	public static int numPopups;

	private bool showRanks = true;

	public TextMesh matchmakingBattleText;

	private bool triedToDoPrivateMatchingNoGas;

	private List<string> tipList;

	private int currentTip;

	public TextMesh specialOfferName;

	public TextMesh specialOfferCostGas;

	public TextMesh specialOfferCostJoules;

	public Transform[] dealItems;

	public TextMesh packageCost;

	public TextMesh packageDiscount;

	public TextMesh ownedDiscountAmount;

	public TextMesh specialOfferBuyButtonCost;

	public GameObject[] specialOfferGasIcons;

	public GameObject[] specialOfferJouleIcons;

	private Deal currentDeal;

	private int currentDealCost = 100000;

	public Gearup gearup;

	private Stack<string> previousMenuStack;

	private GUIButton androidBackButton;

	private string lastMenu;

	public GUIButton teamBattleButton;

	private bool _levelUpSupressedByNickname;

	private bool _rankUpSupressedByNickname;

	private bool _dailyRewardSupressedByNickname;

	private bool _tutorialSupressedByNickname;

	private bool _windows8ControlsSupressedByNickname;

	private string BBTV_URL = "http://youtube.com/user/skyvutv";

	private string MERCH_URL = "http://sky.vu/bbgstorebutton";

	private string _gameModeFFA;

	private string _gameModeTB;

	private string _gameModePTB;

	private string _gameModeKOTH;

	private string _playerNamePrefix;

	private string _matchingTimeUnavailable;

	private string _matchingConnectionFailedTitle;

	private string _matchingConnectionFailedMessage;

	private string _matchingConnectionFailedCloseButton;

	private string _matchingFetchingWaitTime;

	private string _matchingSearchingForGame;

	private string _androidQuitTitle;

	private string _androidQuitMessage;

	private string _androidQuitConfirm;

	private string _androidQuitDismiss;

	private string _chooseGameHour;

	private string _chooseGameMin;

	private string _chooseGameSec;

	private string _chooseGameTankFull;

	private string _gearupCategoryCharacter;

	private string _gearupCategorySkins;

	private string _gearupCategoryTaunts;

	private string _gearupCategoryPrimary;

	private string _gearupCategorySecondary;

	private string _gearupCategoryMelee;

	private string _gearupCategoryProMode;

	private string _gearupCategoryEquipment;

	private string _gearupCategorySpecial;

	private string _errorInvalidSessionTitle;

	private string _errorInvalidSessionBody;

	private string _errorInvalidSessionButton;

	private string _errorDisconnectedTitle;

	private string _errorDisconnectedBody;

	private string _errorDisconnectedButton;

	private string _errorGameCanceledTitle;

	private string _errorGameCanceledBody;

	private string _errorGameCanceledButton;

	public static MainMenu Instance
	{
		get
		{
			return _instance;
		}
	}

	public Animation CharacterAnimator
	{
		get
		{
			return characterAnimator;
		}
	}

	private event Action<Menu> _beganMenuSwitch;

	public event Action<Menu> BeganMenuSwitch
	{
		add
		{
			this._beganMenuSwitch = (Action<Menu>)Delegate.Combine(this._beganMenuSwitch, value);
		}
		remove
		{
			this._beganMenuSwitch = (Action<Menu>)Delegate.Remove(this._beganMenuSwitch, value);
		}
	}

	private event Action<Menu> _endedMenuSwitch;

	public event Action<Menu> EndedMenuSwitch
	{
		add
		{
			this._endedMenuSwitch = (Action<Menu>)Delegate.Combine(this._endedMenuSwitch, value);
		}
		remove
		{
			this._endedMenuSwitch = (Action<Menu>)Delegate.Remove(this._endedMenuSwitch, value);
		}
	}

	private event Action _matchmakingCompleted;

	public event Action MatchmakingCompleted
	{
		add
		{
			this._matchmakingCompleted = (Action)Delegate.Combine(this._matchmakingCompleted, value);
		}
		remove
		{
			this._matchmakingCompleted = (Action)Delegate.Remove(this._matchmakingCompleted, value);
		}
	}

	private void Awake()
	{
		_instance = this;
		UpdateLocalizedText();
		hasBeenToMainMenuScene = true;
		headIcons = new GameObject[headIconMounts.Length];
		myAnimation = base.animation;
		Preferences.Instance.OnLoad();
		popupCamera = popupRoot.GetComponentInChildren<Camera>();
		popupCamera.enabled = false;
		previousMenuStack = new Stack<string>(5);
		previousMenuStack.Push(string.Empty);
		androidBackButton = new GameObject("BackButtonWrapper").AddComponent<GUIButton>();
		Bootloader.Instance.socialName = PlayerPrefs.GetString("socialName", string.Empty);
		if (Bootloader.Instance.socialName == string.Empty)
		{
			PromptFirstTimeUserName();
		}
		bool flag = PlayerPrefs.GetInt("seenReputationPopup", 0) > 0;
		bool val = false;
		if (!flag)
		{
			ShowReputationPopup();
		}
		if (ServiceManager.Instance != null)
		{
			ServiceManager.Instance.UpdateProperty("BBUAccount", ref val);
			if (ServiceManager.Instance.GotRepReward)
			{
				TryCreatePopup(new SetupPopup(_reputationRewardPopup));
			}
			ServiceManager.Instance.CheckMaliciousPlayer(CheckMaliciousPlayerCallback);
		}
		if (AdManager.Instance != null)
		{
			AdManager.Instance.Menu = this;
		}
	}

	private void UpdateLocalizedText()
	{
		_gameModeFFA = Language.Get("GAME_MODE_FREE_FOR_ALL");
		_gameModeTB = Language.Get("GAME_MODE_TEAM_BATTLE");
		_gameModePTB = Language.Get("GAME_MODE_PLANT_THE_BOMB");
		_gameModeKOTH = Language.Get("GAME_MODE_KING_OF_THE_HILL");
		_playerNamePrefix = Language.Get("PLAYER_NAME_PREFIX");
		_matchingTimeUnavailable = Language.Get("MATCHING_WAIT_TIME_UNAVAILABLE");
		_matchingConnectionFailedTitle = Language.Get("MATCHING_WAIT_CONNECTION_FAILED_TITLE");
		_matchingConnectionFailedMessage = Language.Get("MATCHING_WAIT_CONNECTION_FAILED_MESSAGE");
		_matchingConnectionFailedCloseButton = Language.Get("MATCHING_WAIT_CONNECTION_FAILED_CLOSE_BUTTON_LABEL");
		_matchingFetchingWaitTime = Language.Get("MATCHING_WAIT_FETCHING_WAIT_TIME");
		_matchingSearchingForGame = Language.Get("MATCHING_WAIT_SEARCHING_FOR_GAME");
		_androidQuitTitle = Language.Get("ANDROID_QUIT_ALERT_TITLE");
		_androidQuitMessage = Language.Get("ANDROID_QUIT_ALERT_MESSAGE");
		_androidQuitConfirm = Language.Get("ANDROID_QUIT_ALERT_CONFIRM");
		_androidQuitDismiss = Language.Get("ANDROID_QUIT_ALERT_DISMISS");
		_chooseGameHour = Language.Get("CHOOSE_GAME_HOUR");
		_chooseGameMin = Language.Get("CHOOSE_GAME_MIN");
		_chooseGameSec = Language.Get("CHOOSE_GAME_SEC");
		_chooseGameTankFull = Language.Get("CHOOSE_GAME_TANK_IS_FULL");
		_gearupCategoryCharacter = Language.Get("GEARUP_CATEGORY_CHARACTER");
		_gearupCategoryEquipment = Language.Get("GEARUP_CATEGORY_EQUIPMENT");
		_gearupCategoryMelee = Language.Get("GEARUP_CATEGORY_MELEE");
		_gearupCategoryPrimary = Language.Get("GEARUP_CATEGORY_PRIMARY");
		_gearupCategoryProMode = Language.Get("GEARUP_CATEGORY_PRO_MODE");
		_gearupCategorySecondary = Language.Get("GEARUP_CATEGORY_SECONDARY");
		_gearupCategorySkins = Language.Get("GEARUP_CATEGORY_SKINS");
		_gearupCategorySpecial = Language.Get("GEARUP_CATEGORY_SPECIAL");
		_gearupCategoryTaunts = Language.Get("GEARUP_CATEGORY_TAUNTS");
		_errorInvalidSessionTitle = Language.Get("ERROR_INVALID_SESSION_TITLE");
		_errorInvalidSessionBody = Language.Get("ERROR_INVALID_SESSION_BODY");
		_errorInvalidSessionButton = Language.Get("ERROR_INVALID_SESSION_BUTTON_LABEL");
		_errorDisconnectedTitle = Language.Get("ERROR_DISCONNECTED_TITLE");
		_errorDisconnectedBody = Language.Get("ERROR_DISCONNECTED_BODY");
		_errorDisconnectedButton = Language.Get("ERROR_DISCONNECTED_BUTTON_LABEL");
		_errorGameCanceledTitle = Language.Get("ERROR_GAME_CANCELED_TITLE");
		_errorGameCanceledBody = Language.Get("ERROR_GAME_CANCELED_BODY");
		_errorGameCanceledButton = Language.Get("ERROR_GAME_CANCELED_BUTTON_LABEL");
	}

	private void CheckMaliciousPlayerCallback(bool isMalicious)
	{
		if (isMalicious)
		{
			TryCreatePopup(new SetupPopup(_maliciousPlayerPopup));
		}
	}

	private void Update()
	{
		if (!isMatchmaking && Input.GetKeyDown(KeyCode.Escape) && popupRoot.childCount < 2)
		{
			PreviousMenu();
		}
		else if (isMatchmaking && Input.GetKeyDown(KeyCode.Escape))
		{
			ServiceManager.Instance.CancelRequestGame();
		}
	}

	private void ShowReputationPopup()
	{
		TryCreatePopup(new SetupPopup(_reputationPopup));
	}

	private void showCurrentTip()
	{
		if (currentTip > tipList.Count - 1)
		{
			currentTip = 0;
		}
		if (currentTip < 0)
		{
			currentTip = tipList.Count - 1;
		}
		PlayerPrefs.SetInt("currentTip", currentTip);
	}

	private IEnumerator delayedInitialTip(float delay)
	{
		yield return new WaitForSeconds(delay);
		showCurrentTip();
	}

	private void Start()
	{
		tipList = ServiceManager.Instance.GetTips();
		ServiceManager.Instance.UpdateProperty("display_rank", ref showRanks);
		for (int i = 0; i < rankTexts.Length; i++)
		{
			rankTexts[i].renderer.enabled = showRanks;
		}
		Bootloader.Instance.permitItemModifiers = true;
		if (isFirstTime)
		{
			if (tipList.Count > 0)
			{
				currentTip = PlayerPrefs.GetInt("currentTip", -1) + 1;
			}
			numTimesPlayed = PlayerPrefs.GetInt("numTimesLoggedIn", 0);
			numTimesPlayed++;
		}
		if (numTimesPlayed > 0)
		{
			int dailyReward = ServiceManager.Instance.GetDailyReward();
			if (dailyReward != -1)
			{
				if (PlayerNicknamePopupManager.Instance.PopupBeingShown)
				{
					_dailyRewardSupressedByNickname = true;
				}
				else
				{
					TryCreatePopup(new SetupPopup(dailyRewardPopup));
				}
			}
		}
		if (!PlayerPrefs.HasKey("firstTimeApp"))
		{
			PlayerPrefs.SetString("firstTimeApp", "NO");
			if (PlayerNicknamePopupManager.Instance.PopupBeingShown)
			{
				_tutorialSupressedByNickname = true;
			}
		}
		if (isFirstTime)
		{
			CumulativeStats.Instance.OnLoadStats();
			ServiceManager.Instance.RefreshGooglePlayProductList(null, null);
		}
		if (PlayerPrefs.HasKey("played_once"))
		{
			if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
			{
				gameChooseScroller.OnSetIndex(1);
			}
			else if (Preferences.Instance.CurrentGameMode == GameMode.TB)
			{
				gameChooseScroller.OnSetIndex(2);
			}
			else if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
			{
				gameChooseScroller.OnSetIndex(3);
			}
			else if (Preferences.Instance.CurrentGameMode == GameMode.CTF)
			{
				gameChooseScroller.OnSetIndex(4);
			}
		}
		else
		{
			gameChooseScroller.OnSetIndex(2);
		}
		myAnimation["start_main"].speed = 3f;
		myAnimation["end_chooseGame"].speed = 2f;
		if (!isFirstTime)
		{
			currentMenuAnimation = "main_gearUp_fromGame";
			previousMenuStack.Push("main_gearUp");
		}
		StartCoroutine(switchToMenu(currentMenuAnimation));
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			LoadoutManager.Instance.LoadLastLoadout(ServiceManager.Instance.GetStats().pid);
		}
		loadCharacter(LoadoutManager.Instance.CurrentLoadout);
		if (!isFirstTime)
		{
			(characterMountpoint.GetComponentInChildren(typeof(TouchRotator)) as TouchRotator).OnSetTarget(360f);
		}
		int @int = PlayerPrefs.GetInt("lastLevel" + ServiceManager.Instance.GetStats().pid, 1);
		int num = (int)ServiceManager.Instance.GetStats().level;
		if (@int < num && CumulativeStats.Instance.numGamesPlayed > 0)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("age_in_minutes", (int)(ServiceManager.Instance.GetStats().age_in_minutes / 5.0));
			PlayerPrefs.SetInt("lastLevel" + ServiceManager.Instance.GetStats().pid, (int)ServiceManager.Instance.GetStats().level);
			Bootloader.Instance.reportScore(GameCenterIDDictionaries.Leaderboards["LEVEL"], (int)ServiceManager.Instance.GetStats().level);
			if (PlayerNicknamePopupManager.Instance.PopupBeingShown)
			{
				_levelUpSupressedByNickname = true;
			}
			else
			{
				TryCreatePopup(new SetupPopup(levelupPopup));
			}
		}
		if (wasDisconnected)
		{
			ServiceManager.Instance.LogGameLeft("disconnected");
			createErrorMessage(_errorDisconnectedTitle, _errorDisconnectedBody, _errorDisconnectedButton, string.Empty);
		}
		else if (gameCancelled)
		{
			createErrorMessage(_errorGameCanceledTitle, _errorGameCanceledBody, _errorDisconnectedButton, string.Empty);
		}
		else if (hasInvalidSession)
		{
			OnSessionError();
		}
		wasDisconnected = false;
		gameCancelled = false;
		isFirstTime = false;
		hasInvalidSession = false;
		if (!ServiceManager.Instance.GetStats().guest)
		{
			createAccountButton.SetActive(false);
		}
		OnRefresh();
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		StartCoroutine(TryShowFeaturedOffer());
	}

	private IEnumerator TryShowFeaturedOffer()
	{
		while (!AdManager.Instance.DidInitialize && AdManager.Instance.HaveNotReceivedInitCallback)
		{
			yield return null;
		}
		AdManager.Instance.ShowFeaturedOffer();
	}

	private void updateNextRefillTime()
	{
		Stats stats = ServiceManager.Instance.GetStats();
		for (int i = 0; i < energyBars.Length; i++)
		{
			energyBars[i].transform.localScale = new Vector3((float)stats.games_left / (float)stats.max_games, 1f, 1f);
		}
		StopCoroutine("delayedUpdateStats");
		StartCoroutine("delayedUpdateStats", stats.seconds_to_refill);
	}

	private IEnumerator delayedUpdateStats(int secondsUntilRefill)
	{
		Stats playerStats = ServiceManager.Instance.GetStats();
		if (playerStats.games_left >= playerStats.max_games)
		{
			for (int i = 0; i < refillTexts.Length; i++)
			{
				refillTexts[i].text = _chooseGameTankFull;
			}
			yield break;
		}
		while (playerStats.games_left < playerStats.max_games && secondsUntilRefill > 0)
		{
			for (int j = 0; j < refillTexts.Length; j++)
			{
				int hours = secondsUntilRefill / 3600;
				int remainingSeconds = secondsUntilRefill % 3600;
				int minutes = remainingSeconds / 60;
				int seconds = remainingSeconds % 60;
				refillTexts[j].text = string.Format("{0} {1} {2} {3} {4} {5}", hours, _chooseGameHour, minutes, _chooseGameMin, seconds, _chooseGameSec);
			}
			yield return new WaitForSeconds(1f);
			secondsUntilRefill--;
		}
		yield return new WaitForSeconds(2f);
		ServiceManager.Instance.RefreshPlayerStats(OnRefreshSuccess, OnRefreshFail);
	}

	private void OnRefreshSuccess()
	{
		OnRefresh();
	}

	private void OnRefreshFail()
	{
		if (ServiceManager.Instance.LastError == "Invalid session")
		{
			OnSessionError();
		}
	}

	public void OnOutOfGasInTrade(Action<GameObject> setupMethod)
	{
		showBuyGasPage(setupMethod);
	}

	private void showBuyGasPage(Action<GameObject> setupMethod = null)
	{
		TryCreatePopup(new SetupPopup(buyGasPopup, setupMethod));
	}

	public void OnSessionError()
	{
		createErrorMessage(_errorInvalidSessionTitle, _errorInvalidSessionBody, _errorInvalidSessionButton, "Login");
	}

	public void OnUpdatePlayerStats()
	{
		Stats stats = ServiceManager.Instance.GetStats();
		for (int i = 0; i < levelTexts.Length; i++)
		{
			levelTexts[i].text = ((int)stats.level).ToString();
		}
		for (int j = 0; j < levelChargebars.Length; j++)
		{
			levelChargebars[j].localScale = new Vector3((float)(stats.level - (double)(int)stats.level), 1f, 1f);
		}
		updatePlayerGas();
		updatePlayerJoules();
		updatePlayerRank();
	}

	private void updatePlayerRank()
	{
		int @int = PlayerPrefs.GetInt("previousSkill_v2" + ServiceManager.Instance.GetStats().pid, 0);
		Rank int2 = (Rank)PlayerPrefs.GetInt("previousRank_v2" + ServiceManager.Instance.GetStats().pid, 0);
		Stats stats = ServiceManager.Instance.GetStats();
		Rank rank = ServiceManager.GetRank(stats.skill);
		int int3 = PlayerPrefs.GetInt("lastRankShownGamesPlayed_v2" + ServiceManager.Instance.GetStats().pid, 0);
		if ((double)@int < stats.skill && int2 != rank && CumulativeStats.Instance.numGamesPlayed > int3)
		{
			PlayerPrefs.SetFloat("previousSkill_v2" + ServiceManager.Instance.GetStats().pid, (float)stats.skill);
			PlayerPrefs.SetInt("previousRank_v2" + ServiceManager.Instance.GetStats().pid, (int)rank);
			PlayerPrefs.SetInt("lastRankShownGamesPlayed_v2" + ServiceManager.Instance.GetStats().pid, CumulativeStats.Instance.numGamesPlayed);
			int2 = rank;
			Rank int4 = (Rank)PlayerPrefs.GetInt("highestEverRank_v2" + ServiceManager.Instance.GetStats().pid, 0);
			if (rank > int4)
			{
				PlayerPrefs.SetInt("highestEverRank_v2" + ServiceManager.Instance.GetStats().pid, (int)rank);
				if (PlayerNicknamePopupManager.Instance.PopupBeingShown)
				{
					_rankUpSupressedByNickname = true;
				}
				else
				{
					TryCreatePopup(new SetupPopup(rankupPopup));
				}
			}
		}
		string text = Enum.GetName(typeof(Rank), (int)rank);
		UnityEngine.Object @object = Resources.Load("Icons/Rank/" + text);
		if (@object != null)
		{
			for (int i = 0; i < rankIconMounts.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = rankIconMounts[i];
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		foreach (object item in medalRankRoot)
		{
			((Transform)item).gameObject.SetActive(false);
		}
		medalRankRoot.Find(text).gameObject.SetActive(true);
		foreach (object item2 in largeMedalRoot)
		{
			((Transform)item2).gameObject.SetActive(false);
		}
		largeMedalRoot.Find(text).gameObject.SetActive(true);
		for (int j = 0; j < rankTexts.Length; j++)
		{
			rankTexts[j].text = ((int)stats.skill).ToString();
		}
	}

	private void updatePlayerGas()
	{
		Stats stats = ServiceManager.Instance.GetStats();
		string text = string.Format("{0:#,0}", stats.gas);
		for (int i = 0; i < currentGasVisuals.Length; i++)
		{
			currentGasVisuals[i].text = text;
		}
	}

	private void updatePlayerJoules()
	{
		Stats stats = ServiceManager.Instance.GetStats();
		string text = string.Format("{0:#,0}", stats.joules);
		for (int i = 0; i < currentJoulesVisuals.Length; i++)
		{
			currentJoulesVisuals[i].text = text;
		}
	}

	public void updateHeadIcons()
	{
		for (int i = 0; i < headIcons.Length; i++)
		{
			if (headIcons[i] != null)
			{
				UnityEngine.Object.Destroy(headIcons[i]);
			}
		}
		PlayerLoadout currentLoadout = LoadoutManager.Instance.CurrentLoadout;
		UnityEngine.Object @object = Resources.Load("Icons/Characters/" + currentLoadout.model.name + "/" + currentLoadout.skin.name + "_red");
		if (@object != null)
		{
			for (int j = 0; j < headIconMounts.Length; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = headIconMounts[j];
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				headIcons[j] = gameObject;
			}
		}
	}

	public void OnContinueWithoutEnergy()
	{
		if (!triedToDoPrivateMatchingNoGas)
		{
			previousMenuStack.Push("chooseGame_play");
			StartCoroutine(switchToMenu("chooseGame_play"));
		}
		else
		{
			TryCreatePopup(new SetupPopup(privateMatchPopup));
		}
	}

	private IEnumerator switchToMenu(string menuAnimation)
	{
		if (myAnimation[menuAnimation] != null)
		{
			if (menuAnimation.Equals("gearUp_spOffers"))
			{
				loadCharacter(LoadoutManager.Instance.CurrentLoadout);
			}
			string[] split = menuAnimation.Split('_');
			if (split.Length >= 2 && split[1].Contains("main"))
			{
				previousMenuStack.Clear();
				previousMenuStack.Push(string.Empty);
			}
			if (menuAnimation != "gearUp_customize" && menuAnimation != "customize_gearUp")
			{
				switch (menuAnimation)
				{
				case "main_spOffers":
				case "main_gearUp":
				case "chooseGame_main":
				case "play_chooseGame":
				case "gearUp_spOffers":
				case "gearUp_chooseGame":
				case "gearUp_main":
				case "stats_main":
					if (rotateRightSound != null)
					{
						AudioSource.PlayClipAtPoint(rotateRightSound, Vector3.zero);
					}
					break;
				default:
					if (rotateLeftSound != null)
					{
						AudioSource.PlayClipAtPoint(rotateLeftSound, Vector3.zero);
					}
					break;
				}
			}
			string[] buttonSplit = menuAnimation.Split('_');
			string currBtnMenu = buttonSplit[0];
			string nextBtnMenu = buttonSplit[1];
			TrackMenuSwitch(currBtnMenu, nextBtnMenu);
			RaiseBeganMenuSwitchEvent(nextBtnMenu);
			if (nextBtnMenu == "play")
			{
				ForcePlayerXOffset(-72f);
			}
			else if (currBtnMenu != "play")
			{
				ForcePlayerXOffset(0f);
			}
			guiController.IsActive = false;
			myAnimation.Play(menuAnimation);
			Transform newText = textRoot.Find(nextBtnMenu);
			if (newText != null)
			{
				newText.gameObject.SetActiveRecursively(true);
			}
			menuAreas.SetAreaActive(nextBtnMenu);
			if (menuAnimation == "chooseGame_play")
			{
				characterMountpoint.transform.localScale = Vector3.zero;
				yield return new WaitForSeconds(0.3f);
				characterMountpoint.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
				if (myAnimation[menuAnimation].speed > 0f)
				{
					yield return new WaitForSeconds(myAnimation[menuAnimation].length / myAnimation[menuAnimation].speed - 0.3f);
				}
			}
			else if (myAnimation[menuAnimation].speed > 0f)
			{
				yield return new WaitForSeconds(myAnimation[menuAnimation].length / myAnimation[menuAnimation].speed);
			}
			if (numPopups == 0)
			{
				guiController.IsActive = true;
			}
			menuColliders.SetGroupActive(nextBtnMenu);
			menuAreas.SetOtherAreasInactive(nextBtnMenu);
			Transform previousText = textRoot.Find(currBtnMenu);
			if (previousText != null)
			{
				previousText.gameObject.SetActiveRecursively(false);
			}
			currentMenuAnimation = menuAnimation;
			if (currentMenuAnimation == "chooseGame_play")
			{
				if (!isMatchmaking)
				{
					averageWaitTimeText.text = _matchingFetchingWaitTime;
					userNumberText.text = _matchingSearchingForGame;
					timeElapsed.text = "0:00";
					startMatchmaking(matchChoiceIndex);
					StartCoroutine(showMatchmakingTimeElapsed(Preferences.Instance.CurrentGameMode == GameMode.ROYL && LoadoutManager.Instance.CurrentLoadout.model.name == "Saberi"));
				}
			}
			else if (currentMenuAnimation == "play_chooseGame")
			{
				characterMountpoint.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			RaiseEndedMenuSwitchEvent(nextBtnMenu);
		}
		switch (menuAnimation)
		{
		case "gearUp_main":
		case "chooseGame_main":
		case "stats_main":
			if (tipList.Count > 0)
			{
				currentTip = PlayerPrefs.GetInt("currentTip", -1) + 1;
			}
			break;
		}
	}

	private void ForcePlayerXOffset(float targetX)
	{
		if (characterMountpoint.childCount > 0)
		{
			Vector3 localPosition = characterMountpoint.localPosition;
			localPosition.x = targetX;
			characterMountpoint.localPosition = localPosition;
		}
	}

	private void TrackMenuSwitch(string currBtnMenu, string nextBtnMenu)
	{
		switch (nextBtnMenu)
		{
		case "main":
			EventTracker.TrackEvent(new MainMenuOpenedSchema());
			break;
		case "gearUp":
			if (currBtnMenu != "customize")
			{
				EventTracker.TrackEvent(new GearUpOpenedSchema());
			}
			break;
		case "spOffers":
			EventTracker.TrackEvent(new BundleDealsOpenedSchema());
			break;
		case "chooseGame":
			EventTracker.TrackEvent(new MatchSelectOpenedSchema());
			break;
		case "play":
			EventTracker.TrackEvent(new MatchmakingEnteredSchema());
			break;
		}
	}

	private void RaiseBeganMenuSwitchEvent(string nextBtnMenu)
	{
		if (this._beganMenuSwitch != null)
		{
			this._beganMenuSwitch(MenuForButtonString(nextBtnMenu));
		}
	}

	private void RaiseEndedMenuSwitchEvent(string nextBtnMenu)
	{
		if (this._endedMenuSwitch != null)
		{
			this._endedMenuSwitch(MenuForButtonString(nextBtnMenu));
		}
	}

	private Menu MenuForButtonString(string nextBtnMenu)
	{
		switch (nextBtnMenu)
		{
		case "main":
			return Menu.Main;
		case "spOffers":
			return Menu.BundleDeals;
		case "spBuy":
			return Menu.BuyBundle;
		case "customize":
			return Menu.Customize;
		case "gearUp":
			return Menu.GearUp;
		case "chooseGame":
			return Menu.MatchSelect;
		case "play":
			return Menu.Matchmaking;
		default:
			throw new Exception("No Menu case for " + nextBtnMenu);
		}
	}

	private IEnumerator showMatchmakingTimeElapsed(bool randomizeCharacter)
	{
		int secondsElapsed = 0;
		List<Item> allOwnedAndAllowedCharacters = (from i in ServiceManager.Instance.GetItemsForType(Item.Types.character.ToString())
			where i.name != "Saberi" && ServiceManager.Instance.IsItemBought(i.id)
			select i).ToList();
		allOwnedAndAllowedCharacters.Shuffle();
		while (isMatchmaking && timeElapsed != null)
		{
			if (randomizeCharacter)
			{
				RandomizeCharacter(allOwnedAndAllowedCharacters, secondsElapsed, ref randomizeCharacter);
			}
			if (secondsElapsed % 10 == 0)
			{
				if (motds.Length > 0)
				{
					messageOfTheDay.text = motds[UnityEngine.Random.Range(0, motds.Length)];
				}
				else
				{
					messageOfTheDay.text = "No message of the day today";
				}
			}
			if (secondsElapsed < 60)
			{
				if (secondsElapsed < 10)
				{
					timeElapsed.text = "0:0" + secondsElapsed;
				}
				else
				{
					timeElapsed.text = "0:" + secondsElapsed;
				}
			}
			else
			{
				int minutes = secondsElapsed / 60;
				int seconds = secondsElapsed % 60;
				if (seconds < 10)
				{
					timeElapsed.text = minutes + ":0" + seconds;
				}
				else
				{
					timeElapsed.text = minutes + ":" + seconds;
				}
			}
			yield return new WaitForSeconds(1f);
			secondsElapsed++;
		}
	}

	private void RandomizeCharacter(IList<Item> allOwnedAndAllowedCharacters, int secondsElapsed, ref bool randomizeCharacter)
	{
		string model = allOwnedAndAllowedCharacters[secondsElapsed % allOwnedAndAllowedCharacters.Count].name;
		PlayerLoadout playerLoadout = LoadoutManager.Instance.LoadLoadout(ServiceManager.Instance.GetStats().pid, model, LoadoutManager.Instance.CurrentLoadout.loadoutNumber);
		if (!isLoadingLevel)
		{
			if (secondsElapsed % 3 == 0)
			{
				loadCharacter(playerLoadout);
			}
		}
		else
		{
			LoadoutManager.Instance.CurrentLoadout = playerLoadout;
			randomizeCharacter = false;
		}
	}

	public void OnCreateBuyGasPopup(Action<GameObject> setupMethod = null)
	{
		showBuyGasPage(setupMethod);
	}

	public void OnCreateBuyJoulesPopup(Action<GameObject> setupMethod)
	{
		TryCreatePopup(new SetupPopup(buyJoulesPopup, setupMethod));
	}

	public void OnShowItemDescription(GameObject popup, Item i)
	{
		if (i != null && i.description != string.Empty)
		{
			TryCreatePopup(new SetupPopup(popup));
			currentPopup.SendMessage("OnShowItem", i, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnGetRewardSuccess(RewardableActionNameParameter.Action rewardableActionName, Stats prevStats)
	{
		EventTracker.TrackEvent(new RewardGrantSucceededSchema(new RewardableActionNameParameter(rewardableActionName), ParamsFromCurrencyThatChanged(prevStats, ServiceManager.Instance.GetStats())));
		OnGetRewardSuccess();
	}

	public void OnGetRewardSuccess()
	{
		specialOffersManager.updateSpecialOffers();
		updatePlayerGas();
		updatePlayerJoules();
	}

	private VirtualCurrencyParameters ParamsFromCurrencyThatChanged(Stats prevStats, Stats currStats)
	{
		int num = 0;
		VirtualCurrencyNameParameter.CurrencyName currency;
		if (prevStats.gas != currStats.gas)
		{
			num = currStats.gas;
			currency = VirtualCurrencyNameParameter.CurrencyName.GAS;
		}
		else
		{
			num = currStats.joules;
			currency = VirtualCurrencyNameParameter.CurrencyName.JOULES;
		}
		return new VirtualCurrencyParameters(new VirtualCurrencyAmountParameter(num), new VirtualCurrencyNameParameter(currency), new VirtualCurrencyTypeParameter(currency));
	}

	private void OnGetRewardFail(RewardableActionNameParameter.Action rewardableActionName)
	{
		EventTracker.TrackEvent(new RewardGrantFailedSchema(new RewardableActionNameParameter(rewardableActionName), new RewardGrantErrorParameter(ServiceManager.Instance.LastError)));
		OnGetRewardFail();
	}

	public void OnGetRewardFail()
	{
		Debug.LogWarning("reward fail: " + ServiceManager.Instance.LastError);
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		Stats stats = ServiceManager.Instance.GetStats();
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[UnityEngine.Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		if (b.name != "buyEquipmentSlot")
		{
			accessDenied.SetActive(false);
		}
		if (!HandlePlayNowButton(b) && !HandleCancelMatchmakingButton(b, stats) && !HandleGameTipButton(b, stats))
		{
			HandleSpecialOfferButton(b, stats);
			HandleTryPlayWithNoGamesLeft(b, stats);
			SetCharacterRotatorTarget(b);
			if (!HandleBackButton(b, stats) && !HandleGearUpCustomizationPressed(b, stats) && !HandleMainMenuOptionsButton(b, stats) && !HandleMainMenuExtrasButton(b, stats) && !HandleGetGasButton(b, stats) && !HandleGetJoulesButton(b, stats) && !HandleCreateAccountMenuButton(b, stats) && !HandleBuyCurrentDealButton(b, stats) && !HandlePrivateMatchButton(b, stats) && !HandleDiscordButton(b))
			{
				HandleSwitchMenuFromUnderscoreName(b, stats);
				HandleMatchmakingStartButtons(b, stats);
			}
		}
	}

	private bool HandlePlayNowButton(GUIButton b)
	{
		if (b.name.StartsWith("main_play"))
		{
			SetQuickPlayGameModeAndMatchmakingText(ParseGameModeFromButton(b.name));
			this.StartCoroutine(switchToMenu("main_chooseGame"), delegate
			{
				StartCoroutine(switchToMenu("chooseGame_play"));
			});
			return true;
		}
		if (b.name.StartsWith("gearUp_chooseGame"))
		{
			SetQuickPlayGameModeAndMatchmakingText(ParseGameModeFromButton(b.name));
			this.StartCoroutine(switchToMenu("gearUp_chooseGame"), delegate
			{
				StartCoroutine(switchToMenu("chooseGame_play"));
			});
			return true;
		}
		return false;
	}

	private GameMode ParseGameModeFromButton(string buttonName)
	{
		return (GameMode)(int)Enum.Parse(typeof(GameMode), buttonName.Split('_')[2]);
	}

	private void SetQuickPlayGameModeAndMatchmakingText(GameMode gameMode)
	{
		Preferences.Instance.CurrentGameMode = gameMode;
		matchmakingBattleText.text = MatchmakingTextForGameMode(gameMode);
	}

	private string MatchmakingTextForGameMode(GameMode gameMode)
	{
		switch (gameMode)
		{
		case GameMode.FFA:
			return _gameModeFFA;
		case GameMode.TB:
			return _gameModeTB;
		case GameMode.CTF:
			return _gameModePTB;
		case GameMode.KOTH:
			return _gameModeKOTH;
		case GameMode.ROYL:
			return "Royale";
		default:
			throw new Exception("No matchmaking text case defined for game mode " + gameMode);
		}
	}

	private bool HandleGameTipButton(GUIButton b, Stats stats)
	{
		if (b.name == "nextTip")
		{
			currentTip++;
			showCurrentTip();
			return true;
		}
		if (b.name == "previousTip")
		{
			currentTip--;
			showCurrentTip();
			return true;
		}
		return false;
	}

	private bool HandleCancelMatchmakingButton(GUIButton b, Stats stats)
	{
		if (b.name == "play_chooseGame")
		{
			ServiceManager.Instance.CancelRequestGame();
			return true;
		}
		return false;
	}

	private void HandleSpecialOfferButton(GUIButton b, Stats stats)
	{
		if (!b.name.StartsWith("chooseOffer"))
		{
			return;
		}
		int num = int.Parse(b.name[b.name.Length - 1].ToString());
		if (num < specialOffersManager.specialOfferButtons.Count)
		{
			string text = specialOffersManager.specialOfferButtons[num].name;
			if (text.StartsWith("deal_"))
			{
				int dealID = int.Parse(text.Split('_')[1]);
				currentDeal = ServiceManager.Instance.GetDeal(dealID);
				displayDeal(currentDeal);
				EventTracker.TrackEvent(new BundleDetailsOpenedSchema(new ItemNameParameter(currentDeal.name)));
				return;
			}
			if (text == "freegas_btn")
			{
				AdManager.Instance.ShowAd(AdType.storeOfferwall);
				EventTracker.TrackEvent(new OfferwallOpenedSchema(new CurrentGasParameter(stats.gas)));
				return;
			}
			RewardableActionNameParameter.Action rewardableActionName;
			switch (text)
			{
			case "facebook_like":
				Application.OpenURL("fb://profile/193480350173");
				rewardableActionName = RewardableActionNameParameter.Action.FACEBOOK;
				break;
			case "twitter_follow":
				Application.OpenURL("twitter://user?screen_name=battlebears");
				rewardableActionName = RewardableActionNameParameter.Action.TWITTER;
				break;
			case "youtube_subscribe":
				EtceteraAndroid.showWebView("http://www.youtube.com/c/BattleBears");
				rewardableActionName = RewardableActionNameParameter.Action.YOUTUBE;
				break;
			case "website_visit":
				EtceteraAndroid.showWebView("http://sky.vu/forums");
				rewardableActionName = RewardableActionNameParameter.Action.BATTLEBEARS_COM;
				break;
			case "bbf_btn":
				EtceteraAndroid.showWebView("http://sky.vu/bbzgp");
				rewardableActionName = RewardableActionNameParameter.Action.OTHER_APP;
				break;
			default:
				throw new Exception("No reward button case defined for " + text);
			}
			EventTracker.TrackEvent(new RewardableActionClickedSchema(new RewardableActionNameParameter(rewardableActionName)));
			if (!ServiceManager.Instance.PlayerHasReward(text))
			{
				ServiceManager.Instance.RequestReward(text, delegate
				{
					OnGetRewardSuccess(rewardableActionName, stats);
				}, delegate
				{
					OnGetRewardFail(rewardableActionName);
				});
			}
		}
		else
		{
			Debug.LogWarning("should never get here in chooseOffer index!");
		}
	}

	private void HandleTryPlayWithNoGamesLeft(GUIButton b, Stats stats)
	{
		if ((b.name == "chooseGame1" || b.name == "chooseGame2" || b.name == "chooseGame3" || b.name == "chooseGame4" || b.name == "privateMatch") && stats.games_left <= 0)
		{
			if (b.name == "chooseGame1")
			{
				Preferences.Instance.CurrentGameMode = GameMode.TB;
				triedToDoPrivateMatchingNoGas = false;
			}
			else if (b.name == "chooseGame2")
			{
				Preferences.Instance.CurrentGameMode = GameMode.FFA;
				matchmakingBattleText.text = _gameModeFFA;
				triedToDoPrivateMatchingNoGas = false;
			}
			else if (b.name == "chooseGame3")
			{
				Preferences.Instance.CurrentGameMode = GameMode.CTF;
				matchmakingBattleText.text = _gameModeTB;
				triedToDoPrivateMatchingNoGas = false;
			}
			else if (b.name == "chooseGame4")
			{
				Preferences.Instance.CurrentGameMode = GameMode.KOTH;
				matchmakingBattleText.text = _gameModePTB;
				triedToDoPrivateMatchingNoGas = false;
			}
			else if (b.name == "privateMatch")
			{
				triedToDoPrivateMatchingNoGas = true;
			}
			TryCreatePopup(new SetupPopup(outOfEnergy));
		}
	}

	private void SetCharacterRotatorTarget(GUIButton b)
	{
		if (b.name == "main_gearUp" || b.name == "customize_gearUp" || b.name == "chooseGame_gearUp" || b.name == "main_stats")
		{
			(characterMountpoint.GetComponentInChildren(typeof(TouchRotator)) as TouchRotator).OnSetTarget(360f);
		}
		if (b.name == "gearUp_main" || b.name == "chooseGame_main" || b.name == "stats_main")
		{
			(characterMountpoint.GetComponentInChildren(typeof(TouchRotator)) as TouchRotator).OnSetTarget(180f);
		}
	}

	private bool HandleBackButton(GUIButton b, Stats stats)
	{
		if (b.name == "back")
		{
			string[] array = currentMenuAnimation.Split('_');
			previousMenuStack.Push(array[1] + "_" + array[0]);
			StartCoroutine(switchToMenu(array[1] + "_" + array[0]));
			return true;
		}
		return false;
	}

	private bool HandleGearUpCustomizationPressed(GUIButton b, Stats stats)
	{
		if (b.name.StartsWith("gearcategory"))
		{
			guiController.IsActive = true;
			StartCoroutine(switchToMenu("gearUp_customize"));
			return true;
		}
		return false;
	}

	private bool HandleMainMenuOptionsButton(GUIButton b, Stats stats)
	{
		if (b.name == "main_options")
		{
			EventTracker.TrackEvent(new MainMenuOptionsOpenedSchema());
			TryCreatePopup(new SetupPopup(optionsPopup, delegate(GameObject go)
			{
				go.GetComponent<PauseMenu>().ClosingCallback = delegate
				{
					EventTracker.TrackEvent(new MainMenuOptionsClosedSchema());
				};
			}));
			return true;
		}
		return false;
	}

	private bool HandleMainMenuExtrasButton(GUIButton b, Stats stats)
	{
		if (b.name == "main_news")
		{
			TryCreatePopup(new SetupPopup(extrasPopup));
			return true;
		}
		return false;
	}

	private bool HandleGetGasButton(GUIButton b, Stats stats)
	{
		if (b.name == "gasJoules_getGas")
		{
			showBuyGasPage();
			return true;
		}
		return false;
	}

	private bool HandleGetJoulesButton(GUIButton b, Stats stats)
	{
		if (b.name == "gasJoules_getJoules")
		{
			TryCreatePopup(new SetupPopup(buyJoulesPopup));
			return true;
		}
		return false;
	}

	private bool HandleCreateAccountMenuButton(GUIButton b, Stats stats)
	{
		if (b.name == "mainMenu_createAcct")
		{
			LoginManager.Instance.OnShowCreateAccountMenu(guiController);
			return true;
		}
		return false;
	}

	private bool HandleBuyCurrentDealButton(GUIButton b, Stats stats)
	{
		if (b.name == "buyOffer_btn")
		{
			int? gas = currentDeal.gas;
			if (gas.HasValue)
			{
				if (ServiceManager.Instance.GetStats().gas >= currentDealCost)
				{
					PurchaseCurrentDeal();
				}
				else
				{
					OpenInsufficientFundsForCurrentDeal(true, stats);
				}
			}
			else if (ServiceManager.Instance.GetStats().joules >= currentDealCost)
			{
				PurchaseCurrentDeal();
			}
			else
			{
				OpenInsufficientFundsForCurrentDeal(fader, stats);
			}
			return true;
		}
		return false;
	}

	private void PurchaseCurrentDeal()
	{
		EventTracker.TrackEvent(DealTransactionEventHelper.Transaction(currentDeal));
		ServiceManager.Instance.PurchaseDeal(currentDeal.id, OnDealBuySuccess, OnDealBuyFail);
	}

	private void OpenInsufficientFundsForCurrentDeal(bool isGas, Stats stats)
	{
		int? num = ((!isGas) ? currentDeal.joules : currentDeal.gas);
		VirtualCurrencyNameParameter.CurrencyName currency = ((!isGas) ? VirtualCurrencyNameParameter.CurrencyName.JOULES : VirtualCurrencyNameParameter.CurrencyName.GAS);
		EventTracker.TrackEvent(new InsufficientFundsOpenedSchema(new ItemNameParameter(currentDeal.name), new VirtualCurrencyParameters(new VirtualCurrencyAmountParameter(num.Value), new VirtualCurrencyNameParameter(currency), new VirtualCurrencyTypeParameter(currency)), (!isGas) ? null : new CurrentGasParameter(stats.joules), isGas ? null : new CurrentJoulesParameter(stats.joules)));
		gearup.createShopPopup(tradeGasShopPopup, num.Value, isGas);
	}

	private bool HandlePrivateMatchButton(GUIButton b, Stats stats)
	{
		if (b.name == "privateMatch")
		{
			TryCreatePopup(new SetupPopup(privateMatchPopup));
			return true;
		}
		return false;
	}

	private bool HandleDiscordButton(GUIButton b)
	{
		if (b.name == "webDiscord")
		{
			Application.OpenURL("https://discord.gg/BB");
			return true;
		}
		return false;
	}

	private void HandleSwitchMenuFromUnderscoreName(GUIButton b, Stats stats)
	{
		if (b.name.Contains("_") && lastMenu != b.name && b.name != "customize_gearUp")
		{
			previousMenuStack.Push(b.name);
		}
		StartCoroutine(switchToMenu(b.name));
	}

	private void HandleMatchmakingStartButtons(GUIButton b, Stats stats)
	{
		switch (b.name)
		{
		case "chooseGame0":
			EventTracker.TrackEvent(new TutorialStartedSchema());
			Application.LoadLevel("Tutorial");
			Bootloader.Instance.InTutorial = true;
			break;
		case "chooseGame1":
			HandleMatchakingSelection(GameMode.ROYL, "Royale");
			break;
		case "chooseGame2":
			HandleMatchakingSelection(GameMode.TB, _gameModeTB);
			break;
		case "chooseGame3":
			HandleMatchakingSelection(GameMode.FFA, _gameModeFFA);
			break;
		case "chooseGame4":
			HandleMatchakingSelection(GameMode.CTF, _gameModePTB);
			break;
		case "PLAY_TEMP0":
		case "PLAY_TEMP1":
		case "PLAY_TEMP2":
			matchChoiceIndex = 0;
			break;
		}
	}

	private void HandleMatchakingSelection(GameMode gameMode, string battleText)
	{
		Preferences.Instance.CurrentGameMode = gameMode;
		Bootloader.Instance.InTutorial = false;
		matchmakingBattleText.text = battleText;
		MogaPopUpHandler.ShowTutorialWithCallBack(delegate
		{
			OnContinueWithoutEnergy();
		});
	}

	private void PromptFirstTimeUserName()
	{
		PlayerNicknamePopupManager instance = PlayerNicknamePopupManager.Instance;
		instance.NicknameCanceled = (Action)Delegate.Combine(instance.NicknameCanceled, new Action(OnUserNamePromptCancelled));
		PlayerNicknamePopupManager instance2 = PlayerNicknamePopupManager.Instance;
		instance2.NicknameSaved = (Action<string>)Delegate.Combine(instance2.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
		PlayerNicknamePopupManager.Instance.ShowNicknamePopupWithInitialName(string.Empty);
	}

	private void OnUserNamePromptFinished(string username)
	{
		PlayerNicknamePopupManager instance = PlayerNicknamePopupManager.Instance;
		instance.NicknameCanceled = (Action)Delegate.Remove(instance.NicknameCanceled, new Action(OnUserNamePromptCancelled));
		PlayerNicknamePopupManager instance2 = PlayerNicknamePopupManager.Instance;
		instance2.NicknameSaved = (Action<string>)Delegate.Remove(instance2.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
		Bootloader.Instance.socialName = username;
		PlayerPrefs.SetString("socialName", username);
		if (Bootloader.Instance.socialName == string.Empty)
		{
			PlayerPrefs.SetString("socialName", _playerNamePrefix + ServiceManager.Instance.GetStats().pid);
		}
		ShowDeferredPopups();
	}

	private void OnUserNamePromptCancelled()
	{
		PlayerNicknamePopupManager instance = PlayerNicknamePopupManager.Instance;
		instance.NicknameCanceled = (Action)Delegate.Remove(instance.NicknameCanceled, new Action(OnUserNamePromptCancelled));
		PlayerNicknamePopupManager instance2 = PlayerNicknamePopupManager.Instance;
		instance2.NicknameSaved = (Action<string>)Delegate.Remove(instance2.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
		string @string = PlayerPrefs.GetString("socialName", string.Empty);
		if (Bootloader.Instance.socialName == string.Empty && @string == string.Empty)
		{
			PlayerPrefs.SetString("socialName", _playerNamePrefix + ServiceManager.Instance.GetStats().pid);
		}
		ShowDeferredPopups();
	}

	private void ShowDeferredPopups()
	{
		if (_levelUpSupressedByNickname)
		{
			_levelUpSupressedByNickname = false;
			TryCreatePopup(new SetupPopup(levelupPopup));
		}
		if (_rankUpSupressedByNickname)
		{
			_rankUpSupressedByNickname = false;
			TryCreatePopup(new SetupPopup(rankupPopup));
		}
		if (_dailyRewardSupressedByNickname)
		{
			_dailyRewardSupressedByNickname = false;
			TryCreatePopup(new SetupPopup(dailyRewardPopup));
		}
		if (_windows8ControlsSupressedByNickname)
		{
			_windows8ControlsSupressedByNickname = false;
			TryCreatePopup(new SetupPopup(windows8ControlPopup));
		}
		if (numPopups > 0)
		{
			popupCamera.enabled = true;
		}
	}

	private void OnDealBuySuccess()
	{
		EventTracker.TrackEvent(DealTransactionEventHelper.PurchaseSucceeded(currentDeal));
		specialOffersManager.updateSpecialOffers();
		StartCoroutine(switchToMenu("spBuy_spOffers"));
		OnUpdatePlayerStats();
	}

	private void OnDealBuyFail()
	{
		EventTracker.TrackEvent(DealTransactionEventHelper.PurchaseFailed(currentDeal));
		if (ServiceManager.Instance.LastError == "Invalid session")
		{
			OnSessionError();
		}
	}

	private void displayDeal(Deal dealToDisplay)
	{
		if (dealToDisplay == null)
		{
			return;
		}
		StartCoroutine(switchToMenu("spOffers_spBuy"));
		specialOfferName.GetComponent<TextProcessor>().OnSetText(dealToDisplay.name);
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		float num4 = 0f;
		float num5 = 0f;
		for (num3 = 0; num3 < Mathf.Min(dealToDisplay.item_ids.Count, 5); num3++)
		{
			dealItems[num3].gameObject.SetActiveRecursively(true);
			int num6 = dealToDisplay.item_ids[num3];
			Item itemByID = ServiceManager.Instance.GetItemByID(num6);
			Purchaseable purchaseableByID = ServiceManager.Instance.GetPurchaseableByID(num6);
			num += (float)purchaseableByID.current_gas;
			num2 += (float)purchaseableByID.current_joules;
			if (ServiceManager.Instance.IsItemBought(num6))
			{
				dealItems[num3].Find("txt_owned").gameObject.SetActive(true);
				num4 += (float)purchaseableByID.current_gas;
				num5 += (float)purchaseableByID.current_joules;
			}
			else
			{
				dealItems[num3].Find("txt_owned").gameObject.SetActive(false);
			}
			UnityEngine.Object original = null;
			if (itemByID.type == "special")
			{
				dealItems[num3].Find("txt_character").GetComponent<TextMesh>().text = _gearupCategorySpecial;
				original = UnityEngine.Object.Instantiate(Resources.Load("Icons/Specials/" + itemByID.name)) as GameObject;
			}
			else if (itemByID.type == "equipment")
			{
				dealItems[num3].Find("txt_character").GetComponent<TextMesh>().text = _gearupCategoryEquipment;
				original = UnityEngine.Object.Instantiate(Resources.Load("Icons/Equipment/" + itemByID.name)) as GameObject;
			}
			else if (itemByID.type == "character")
			{
				Item item = null;
				foreach (Item skin in Store.Instance.characters[itemByID.name].skins)
				{
					if (skin.is_default)
					{
						item = skin;
					}
				}
				dealItems[num3].Find("txt_character").GetComponent<TextMesh>().text = _gearupCategoryCharacter;
				original = Resources.Load("Icons/Characters/" + itemByID.name + "/" + item.name + "_red");
			}
			else if (itemByID.type == "skin")
			{
				Item itemByID2 = ServiceManager.Instance.GetItemByID(itemByID.parent_id);
				if (itemByID2 != null)
				{
					dealItems[num3].Find("txt_character").GetComponent<TextMesh>().text = itemByID2.title;
					original = Resources.Load("Icons/Characters/" + itemByID2.name + "/" + itemByID.name + "_red");
				}
				else
				{
					Debug.LogWarning("something went wrong");
				}
			}
			else
			{
				Item itemByID3 = ServiceManager.Instance.GetItemByID(itemByID.parent_id);
				if (itemByID3 != null)
				{
					dealItems[num3].Find("txt_character").GetComponent<TextMesh>().text = itemByID3.title;
					original = Resources.Load("Icons/Weapons/" + itemByID3.name + "/" + itemByID.name);
				}
				else
				{
					Debug.LogWarning("something went wrong");
				}
			}
			Transform transform = dealItems[num3].transform.Find("offerMount/icon");
			if (transform != null)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.name = "icon";
			gameObject.transform.parent = dealItems[num3].transform.Find("offerMount");
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			dealItems[num3].Find("txt_itemName").SendMessage("OnSetText", itemByID.title);
		}
		for (int i = num3; i < 5; i++)
		{
			dealItems[i].gameObject.SetActiveRecursively(false);
		}
		specialOfferCostGas.text = string.Format("{0:#,0}", Mathf.CeilToInt(num));
		specialOfferCostJoules.text = string.Format("{0:#,0}", Mathf.CeilToInt(num2));
		float num7 = (float)ServiceManager.Instance.GetJoulesExchangeRate(10) / 10f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		int? gas = dealToDisplay.gas;
		if (!gas.HasValue)
		{
			GameObject[] array = specialOfferGasIcons;
			foreach (GameObject gameObject2 in array)
			{
				gameObject2.SetActive(false);
			}
			GameObject[] array2 = specialOfferJouleIcons;
			foreach (GameObject gameObject3 in array2)
			{
				gameObject3.SetActive(true);
			}
			int? joules = dealToDisplay.joules;
			num8 = joules.Value;
			num9 = num2 + num * num7;
			num10 = num5 + num4 * num7;
		}
		else
		{
			GameObject[] array3 = specialOfferGasIcons;
			foreach (GameObject gameObject4 in array3)
			{
				gameObject4.SetActive(true);
			}
			GameObject[] array4 = specialOfferJouleIcons;
			foreach (GameObject gameObject5 in array4)
			{
				gameObject5.SetActive(false);
			}
			int? gas2 = dealToDisplay.gas;
			num8 = gas2.Value;
			num9 = num + num2 / num7;
			num10 = num4 + num5 / num7;
		}
		packageCost.text = string.Format("{0:#,0}", Mathf.CeilToInt(num8));
		float num11 = 100f - num8 / num9 * 100f;
		packageDiscount.text = string.Format("({0:00}% OFF)", num11);
		int num12 = Mathf.FloorToInt(num8 * (num10 / num9));
		if ((float)num12 > 0f)
		{
			ownedDiscountAmount.text = string.Format("-{0:#,0}", num12);
		}
		else
		{
			ownedDiscountAmount.text = "NONE";
		}
		currentDealCost = Mathf.RoundToInt(num8 - (float)num12);
		specialOfferBuyButtonCost.text = string.Format("{0:#,0}", currentDealCost);
	}

	private void OnRefillSuccess()
	{
		CumulativeStats.Instance.numRefills++;
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["THE_LEAKY_FAUCET"]);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["ENDLESS_REFILLS"], (float)CumulativeStats.Instance.numRefills / 25f * 100f);
		Stats stats = ServiceManager.Instance.GetStats();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("level", stats.level);
		dictionary.Add("gas_level", preFillGasLevel);
		ServiceManager.Instance.RefreshPlayerStats(OnRefreshSuccess, OnRefreshFail);
	}

	private void OnJoulesSuccess()
	{
		ServiceManager.Instance.RefreshPlayerStats(OnRefreshSuccess, OnRefreshFail);
	}

	private void OnRefillFail()
	{
		if (ServiceManager.Instance.LastError == "Invalid session")
		{
			OnSessionError();
		}
	}

	public void TryCreatePopup(SetupPopup popup)
	{
		if (currentPopup == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(popup.PopupPrefab) as GameObject;
			gameObject.transform.parent = popupRoot;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			Popup popup2 = gameObject.GetComponent(typeof(Popup)) as Popup;
			if (popup2 != null)
			{
				popup2.OnSetCallingObject(base.gameObject, popupCamera);
				popup2.OnSetGUIControllerToDisable(guiController);
			}
			currentPopup = gameObject;
			if (popup.SetupMethod != null)
			{
				popup.SetupMethod(gameObject);
			}
		}
		else
		{
			popUpQueue.Enqueue(popup);
		}
	}

	public void popupClosed()
	{
		currentPopup = null;
		if (popUpQueue.Count > 0)
		{
			TryCreatePopup(popUpQueue.Dequeue());
		}
	}

	public void createErrorMessage(string errorTitle, string errorBody, string buttonText, string levelName)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.errorMessage) as GameObject;
		gameObject.transform.parent = popupRoot;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		ErrorMessage errorMessage = gameObject.GetComponent(typeof(ErrorMessage)) as ErrorMessage;
		if (errorMessage != null)
		{
			errorMessage.OnSetCallingObject(base.gameObject, popupCamera);
			errorMessage.OnSetGUIControllerToDisable(guiController);
			errorMessage.setErrorText(errorTitle, errorBody, buttonText, levelName);
		}
		currentPopup = gameObject;
	}

	public void OnRefresh()
	{
		OnUpdatePlayerStats();
		updateNextRefillTime();
	}

	public void loadSkin(PlayerLoadout loadout)
	{
		CharacterHandle component = currentCharacter.GetComponent<CharacterHandle>();
		if (!TryLoadSkin("_red", component, loadout))
		{
			string text = loadout.skin.name;
			if (text.Contains("|"))
			{
				int length = text.IndexOf("|");
				text = text.Substring(0, length);
			}
			Material material = Resources.Load("Skins/Default/" + BBRQuality.SkinQuality + "/normal") as Material;
			if (material != null)
			{
				component.Skin = UnityEngine.Object.Instantiate(material) as Material;
			}
			Texture2D texture2D = Resources.Load("Characters/" + loadout.model.name + "/Skins/" + BBRQuality.TextureQuality + "/" + text + "_red") as Texture2D;
			if (texture2D != null)
			{
				component.Skin.mainTexture = texture2D;
			}
		}
	}

	private bool TryLoadSkin(string teamName, CharacterHandle handle, PlayerLoadout playerLoadout)
	{
		Material material = Resources.Load("Skins/" + playerLoadout.model.name + "/" + BBRQuality.SkinQuality + "/" + playerLoadout.skin.name + teamName) as Material;
		if (material == null)
		{
			material = Resources.Load("Skins/" + playerLoadout.model.name + "/" + BBRQuality.SkinQuality + "/" + playerLoadout.skin.name) as Material;
		}
		if (material != null)
		{
			handle.Skin = UnityEngine.Object.Instantiate(material) as Material;
			handle.Skin.mainTexture = Resources.Load("Characters/" + playerLoadout.model.name + "/Skins/" + BBRQuality.TextureQuality + "/" + playerLoadout.skin.name + teamName) as Texture2D;
			return true;
		}
		return false;
	}

	public void OnDelayedRotate()
	{
		StartCoroutine(delayedRotate());
	}

	private IEnumerator delayedRotate()
	{
		yield return new WaitForSeconds(0.01f);
		(characterMountpoint.GetComponentInChildren(typeof(TouchRotator)) as TouchRotator).OnSetTarget(360f);
	}

	public void loadCharacter(PlayerLoadout loadout)
	{
		if (currentCharacter != null)
		{
			currentBackpackWeapon = null;
			currentLefthandWeapon = null;
			currentRighthandWeapon = null;
			UnityEngine.Object.Destroy(currentCharacter);
		}
		UnityEngine.Object @object = Resources.Load("Menu/Characters/" + loadout.model.name);
		if (@object == null)
		{
			@object = Resources.Load("Menu/Characters/Oliver");
		}
		currentCharacter = UnityEngine.Object.Instantiate(@object) as GameObject;
		Vector3 localScale = currentCharacter.transform.localScale;
		currentCharacter.transform.parent = characterMountpoint;
		currentCharacter.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		currentCharacter.transform.localPosition = Vector3.zero;
		currentCharacter.transform.localScale = localScale;
		currentCharacter.name = loadout.model.name;
		loadSkin(loadout);
		characterAnimator = currentCharacter.transform.Find("playerModel").animation;
		Component[] componentsInChildren = currentCharacter.GetComponentsInChildren(typeof(WeaponMountPoint));
		Component[] array = componentsInChildren;
		foreach (Component component in array)
		{
			WeaponMountPoint weaponMountPoint = component as WeaponMountPoint;
			if (weaponMountPoint.mountPoint == WeaponMountPoint.Side.LEFT)
			{
				leftMountpoint = weaponMountPoint.transform;
			}
			else if (weaponMountPoint.mountPoint == WeaponMountPoint.Side.RIGHT)
			{
				rightMountpoint = weaponMountPoint.transform;
			}
			else if (weaponMountPoint.mountPoint == WeaponMountPoint.Side.BACK)
			{
				backMountpoint = weaponMountPoint.transform;
			}
			UnityEngine.Object.Destroy(weaponMountPoint);
		}
		_characterUsesBackpack = loadout.model.name.Equals("B1000");
		if (_characterUsesBackpack)
		{
			SetupWeapon(Resources.Load("Characters/" + loadout.model.name + "/SecondaryWeapons/" + loadout.secondary.name) as GameObject, Gearup.GearupSubmenu.SECONDARY);
			SetupWeapon(Resources.Load("Characters/" + loadout.model.name + "/MeleeWeapons/" + loadout.melee.name) as GameObject, Gearup.GearupSubmenu.MELEE);
			SetupWeapon(Resources.Load("Characters/" + loadout.model.name + "/PrimaryWeapons/" + loadout.primary.name) as GameObject, Gearup.GearupSubmenu.PRIMARY);
		}
		else
		{
			handleNewWeapon(loadout, Gearup.GearupSubmenu.NONE);
		}
	}

	public void handleNewWeapon(PlayerLoadout loadout, Gearup.GearupSubmenu weaponType)
	{
		GameObject gameObject = null;
		int num = 0;
		switch (weaponType)
		{
		case Gearup.GearupSubmenu.NONE:
			num = 0;
			if (loadout.model.name.Equals("Oliver"))
			{
				num = 0;
			}
			else if (loadout.model.name.Equals("Riggs"))
			{
				num = 0;
			}
			else if (loadout.model.name.Equals("Will"))
			{
				num = 0;
			}
			else if (loadout.model.name.Equals("Huggable"))
			{
				num = 2;
			}
			else if (loadout.model.name.Equals("Tillman"))
			{
				num = 2;
			}
			else if (loadout.model.name.Equals("Astoria"))
			{
				num = 0;
			}
			else if (loadout.model.name.Equals("Graham"))
			{
				num = 0;
			}
			break;
		case Gearup.GearupSubmenu.PRIMARY:
			num = 0;
			break;
		case Gearup.GearupSubmenu.SECONDARY:
			num = 1;
			break;
		case Gearup.GearupSubmenu.MELEE:
			num = 2;
			break;
		}
		switch (num)
		{
		case 0:
			gameObject = Resources.Load("Characters/" + loadout.model.name + "/PrimaryWeapons/" + loadout.primary.name) as GameObject;
			break;
		case 1:
			gameObject = Resources.Load("Characters/" + loadout.model.name + "/SecondaryWeapons/" + loadout.secondary.name) as GameObject;
			break;
		case 2:
			gameObject = Resources.Load("Characters/" + loadout.model.name + "/MeleeWeapons/" + loadout.melee.name) as GameObject;
			break;
		default:
			Debug.LogError("typeIndex not found!");
			break;
		}
		if (gameObject != null)
		{
			SetupWeapon(gameObject, weaponType);
		}
	}

	private void SetupWeapon(GameObject weaponToLoad, Gearup.GearupSubmenu weaponType)
	{
		if (currentLefthandWeapon != null)
		{
			if (!(weaponToLoad.name != currentLefthandWeapon.name))
			{
				PlayCharacterIdle(weaponToLoad.name, weaponToLoad);
				if (currentLefthandWeapon.animation != null && currentLefthandWeapon.animation["switch"] != null)
				{
					StartCoroutine("playWeaponSwitch", currentLefthandWeapon.animation);
				}
				return;
			}
			if (!_characterUsesBackpack || weaponType == Gearup.GearupSubmenu.MELEE)
			{
				UnityEngine.Object.Destroy(currentLefthandWeapon);
			}
		}
		if (currentRighthandWeapon != null)
		{
			if (!(weaponToLoad.name != currentRighthandWeapon.name))
			{
				PlayCharacterIdle(weaponToLoad.name, weaponToLoad);
				if (currentRighthandWeapon.animation != null && currentRighthandWeapon.animation["switch"] != null)
				{
					StartCoroutine("playWeaponSwitch", currentLefthandWeapon.animation);
				}
				return;
			}
			if (!_characterUsesBackpack || weaponType == Gearup.GearupSubmenu.PRIMARY)
			{
				UnityEngine.Object.Destroy(currentRighthandWeapon);
			}
		}
		if (currentBackpackWeapon != null)
		{
			if (!(weaponToLoad.name != currentBackpackWeapon.name))
			{
				PlayCharacterIdle(weaponToLoad.name, weaponToLoad);
				return;
			}
			if (!_characterUsesBackpack || weaponType == Gearup.GearupSubmenu.SECONDARY)
			{
				UnityEngine.Object.Destroy(currentBackpackWeapon);
			}
		}
		if (!(weaponToLoad != null))
		{
			return;
		}
		WeaponBase component = weaponToLoad.GetComponent<WeaponBase>();
		MeleeWeapon component2 = weaponToLoad.GetComponent<MeleeWeapon>();
		if (component2 != null && component2.useBothHands)
		{
			if (_characterUsesBackpack)
			{
				if (_attachMeleeToBackpack)
				{
					currentLefthandWeapon = loadWeapon(weaponToLoad, backMountpoint);
				}
				else
				{
					currentLefthandWeapon = loadWeapon(weaponToLoad, leftMountpoint);
				}
			}
			else if (component2.useBothHands)
			{
				currentLefthandWeapon = loadWeapon(weaponToLoad, leftMountpoint);
				if (currentLefthandWeapon.animation != null && currentLefthandWeapon.animation["idle"] != null)
				{
					currentLefthandWeapon.animation["idle"].speed = component.idleAnimationSpeed;
					currentLefthandWeapon.animation.Play("idle");
				}
				currentRighthandWeapon = loadWeapon(weaponToLoad, rightMountpoint);
				if (currentRighthandWeapon.animation != null && currentRighthandWeapon.animation["idle"] != null)
				{
					currentRighthandWeapon.animation["idle"].speed = component.idleAnimationSpeed;
					currentRighthandWeapon.animation.Play("idle");
				}
			}
		}
		else if (component != null)
		{
			StopCoroutine("playWeaponSwitch");
			if (component.hand == WeaponMountPoint.Side.LEFT)
			{
				currentLefthandWeapon = loadWeapon(weaponToLoad, leftMountpoint);
				if (currentLefthandWeapon.animation != null && currentLefthandWeapon.animation["switch"] != null)
				{
					StartCoroutine("playWeaponSwitch", currentLefthandWeapon.animation);
				}
			}
			else if (component.hand == WeaponMountPoint.Side.RIGHT)
			{
				if (_characterUsesBackpack)
				{
					currentRighthandWeapon = loadWeapon(weaponToLoad, rightMountpoint);
					if (currentRighthandWeapon.animation != null && currentRighthandWeapon.animation["switch"] != null)
					{
						StartCoroutine(playWeaponSwitch(currentRighthandWeapon.animation));
					}
				}
				else
				{
					currentLefthandWeapon = loadWeapon(weaponToLoad, rightMountpoint);
					if (currentLefthandWeapon.animation != null && currentLefthandWeapon.animation["switch"] != null)
					{
						StartCoroutine(playWeaponSwitch(currentLefthandWeapon.animation));
					}
				}
			}
			else if (component.hand == WeaponMountPoint.Side.BACK)
			{
				currentBackpackWeapon = loadWeapon(weaponToLoad, backMountpoint);
				if (currentBackpackWeapon.animation != null && currentBackpackWeapon.animation["switch"] != null)
				{
					StartCoroutine(playWeaponSwitch(currentBackpackWeapon.animation));
				}
			}
		}
		else
		{
			StopCoroutine("playWeaponSwitch");
			currentRighthandWeapon = loadWeapon(weaponToLoad, rightMountpoint);
			if (currentRighthandWeapon.animation != null && currentRighthandWeapon.animation["switch"] != null)
			{
				StartCoroutine("playWeaponSwitch", currentRighthandWeapon.animation);
			}
		}
		StopCoroutine("playCharacterSwitch");
		if (component.animation != null && component.animation["switch"] != null)
		{
			StartCoroutine("playCharacterSwitch", component);
		}
		else
		{
			playWeaponIdle(component);
		}
	}

	private GameObject loadWeapon(GameObject weaponToLoad, Transform mountpoint)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(weaponToLoad) as GameObject;
		WeaponBase component = gameObject.GetComponent<WeaponBase>();
		if (component != null && !_characterUsesBackpack && component.isRiggedWeapon && (backMountpoint == null || mountpoint.position != backMountpoint.position))
		{
			Transform transform = currentCharacter.transform.FindChild("playerModel");
			if (transform != null)
			{
				gameObject.transform.parent = transform.transform;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
			}
		}
		else
		{
			gameObject.transform.parent = mountpoint;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localPosition = ((!component.isRiggedWeapon) ? Vector3.zero : component.mountedPosition);
			gameObject.transform.localScale = ((!component.isRiggedWeapon) ? Vector3.one : component.mountedScale);
		}
		gameObject.name = weaponToLoad.name;
		Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(LOD));
		Component[] array = componentsInChildren;
		foreach (Component obj in array)
		{
			UnityEngine.Object.Destroy(obj);
		}
		if (gameObject.name != "stalker")
		{
			UnityEngine.Object.Destroy(gameObject.GetComponentInChildren(typeof(WeaponBase)));
		}
		if (component != null)
		{
			if (component.fireEffect != null)
			{
				UnityEngine.Object.Destroy(component.fireEffect.gameObject);
			}
			if (component.attackEffect != null)
			{
				component.attackEffect.SetActive(false);
			}
			if (component.reloadEffect != null)
			{
				UnityEngine.Object.Destroy(component.reloadEffect.gameObject);
			}
		}
		return gameObject;
	}

	public void ToggleWeapons(bool active)
	{
		if (!_characterUsesBackpack)
		{
			if (currentLefthandWeapon != null)
			{
				currentLefthandWeapon.SetActive(active);
			}
			if (currentRighthandWeapon != null)
			{
				currentRighthandWeapon.SetActive(active);
			}
		}
	}

	private void PlayCharacterIdle(string weaponName, GameObject weapon)
	{
		MeleeWeapon component = weapon.GetComponent<MeleeWeapon>();
		if (component != null && characterAnimator["melee_idle"] != null)
		{
			characterAnimator.CrossFade("melee_idle");
			return;
		}
		if (characterAnimator[weaponName + "_idle"] != null)
		{
			characterAnimator.CrossFade(weaponName + "_idle");
		}
		else if (characterAnimator["secondary_idle"] != null)
		{
			characterAnimator.CrossFade("secondary_idle");
		}
		if (characterAnimator["legs_idle"] != null)
		{
			characterAnimator.CrossFade("legs_idle");
		}
	}

	private IEnumerator playWeaponAttackLoop(WeaponBase weaponInfo)
	{
		characterAnimator[weaponInfo.name + "_fire"].speed = weaponInfo.attackAnimationSpeed;
		while (true)
		{
			if (weaponInfo.isConstantFire)
			{
				characterAnimator[weaponInfo.name + "_fire"].wrapMode = WrapMode.Loop;
				characterAnimator.CrossFade(weaponInfo.name + "_fire");
				yield return new WaitForSeconds(2f);
			}
			else
			{
				characterAnimator[weaponInfo.name + "_fire"].wrapMode = WrapMode.Once;
				characterAnimator.CrossFade(weaponInfo.name + "_fire");
				yield return new WaitForSeconds(characterAnimator[weaponInfo.name + "_fire"].length);
			}
			playWeaponIdle(weaponInfo);
			yield return new WaitForSeconds(5f);
		}
	}

	private void playWeaponIdle(WeaponBase weaponInfo)
	{
		if (characterAnimator["legs_idle"] != null)
		{
			characterAnimator["legs_idle"].layer = 1;
			characterAnimator["legs_idle"].wrapMode = WrapMode.Loop;
			characterAnimator.CrossFade("legs_idle");
		}
		if (weaponInfo.isMelee)
		{
			characterAnimator["melee_idle"].layer = 0;
			characterAnimator["melee_idle"].wrapMode = WrapMode.Loop;
			characterAnimator["melee_idle"].speed = weaponInfo.idleAnimationSpeed;
			characterAnimator.CrossFade("melee_idle");
			return;
		}
		string[] array = weaponInfo.name.Split('_');
		string text = weaponInfo.name + "_idle";
		if (array.Length > 1)
		{
			text = array[1] + "_idle";
		}
		if (characterAnimator[text] == null && characterAnimator["secondary_idle"] != null)
		{
			text = "secondary_idle";
		}
		if (characterAnimator[text] != null)
		{
			characterAnimator[text].layer = 0;
			characterAnimator[text].wrapMode = WrapMode.Loop;
			characterAnimator[text].speed = weaponInfo.idleAnimationSpeed;
			characterAnimator.CrossFade(text);
		}
	}

	private IEnumerator playCharacterSwitch(WeaponBase weaponInfo)
	{
		if (characterAnimator != null && characterAnimator[weaponInfo.name + "_switch"] != null)
		{
			characterAnimator.CrossFade(weaponInfo.name + "_switch", 0.2f);
			yield return new WaitForSeconds(characterAnimator[weaponInfo.name + "_switch"].length);
		}
		playWeaponIdle(weaponInfo);
	}

	private IEnumerator playWeaponSwitch(Animation animator)
	{
		animator.Play("switch");
		yield return new WaitForSeconds(animator["switch"].length);
		if (animator != null)
		{
			animator.CrossFade("idle", 0.2f);
		}
	}

	private IEnumerator backgroundLoadLevel(string levelName)
	{
		CumulativeStats.Instance.OnSaveStats();
		isLoadingLevel = true;
		guiController.IsActive = false;
		SoundManager.Instance.pauseMusic();
		Handheld.Vibrate();
		AudioSource.PlayClipAtPoint(gameStartSound, Vector3.zero);
		matchmakingImageAnimator.OnPlayAnimation(levelName);
		myAnimation.Play("play_startGame");
		float animationDelay = 2.5f;
		(characterMountpoint.GetComponentInChildren(typeof(TouchRotator)) as TouchRotator).OnForceReturnZero();
		if ((currentRighthandWeapon == null || !currentRighthandWeapon.activeInHierarchy) && (currentLefthandWeapon == null || !currentLefthandWeapon.activeInHierarchy) && (currentBackpackWeapon == null || !currentBackpackWeapon.activeInHierarchy))
		{
			handleNewWeapon(LoadoutManager.Instance.CurrentLoadout, Gearup.GearupSubmenu.PRIMARY);
			ToggleWeapons(true);
		}
		yield return new WaitForSeconds(animationDelay);
		if (characterAnimator["legs_forward"] != null)
		{
			characterAnimator["legs_forward"].layer = 1;
			characterAnimator["legs_forward"].wrapMode = WrapMode.Loop;
			characterAnimator["legs_forward"].speed = 2f;
			characterAnimator.Play("legs_forward");
		}
		if (currentRighthandWeapon != null)
		{
			string[] splitString = currentRighthandWeapon.name.Split('_');
			string weaponName = currentRighthandWeapon.name;
			if (splitString.Length >= 2)
			{
				weaponName = splitString[1];
			}
			if (characterAnimator[weaponName + "_run"] != null)
			{
				characterAnimator[weaponName + "_run"].layer = 0;
				characterAnimator[weaponName + "_run"].wrapMode = WrapMode.Loop;
				characterAnimator[weaponName + "_run"].speed = 2f;
				characterAnimator.Play(weaponName + "_run");
			}
			else
			{
				characterAnimator["melee_idle"].layer = 0;
				characterAnimator["melee_idle"].wrapMode = WrapMode.Loop;
				characterAnimator["melee_idle"].speed = 2f;
				characterAnimator.Play("melee_idle");
			}
		}
		yield return new WaitForSeconds(animationDelay - 1f);
		yield return new WaitForSeconds(myAnimation["play_startGame"].length - (animationDelay - 1f));
		isMatchmaking = false;
		string actualLevelName = levelName;
		if (Bootloader.Instance.LEVEL_OVERRIDE != string.Empty)
		{
			actualLevelName = Bootloader.Instance.LEVEL_OVERRIDE;
		}
		MemorySweep.levelToLoad = actualLevelName;
		Application.LoadLevel("MemorySweep");
		yield return null;
	}

	private IEnumerator getStockPrices()
	{
		WWW www = new WWW(ServiceManager.Instance.GetServerURL() + "/quote.json");
		yield return www;
		if (www.error == null)
		{
			StockPrice resp = JsonMapper.ToObject<StockPrice>(www.text);
			ServiceManager.Instance.OverwriteProperty("stock_quote", resp.change);
		}
	}

	private void startMatchmaking(int matchChoiceIndex)
	{
		isMatchmaking = true;
		StartCoroutine(getStockPrices());
		ServiceManager.Instance.RequestGame(matchChoiceIndex, OnMatchSuccess, OnMatchFailure);
		StartCoroutine("getWaitTimes");
	}

	private IEnumerator getWaitTimes()
	{
		while (isMatchmaking && !isLoadingLevel)
		{
			ServiceManager.Instance.RequestWaitTime(ServiceManager.Instance.GetCurrentMatchServerIndex(), OnAvgWaitTimeSuccess, OnAvgWaitTimeFailure);
			yield return new WaitForSeconds(10f);
		}
	}

	private void OnAvgWaitTimeSuccess()
	{
		if (isLoadingLevel)
		{
			return;
		}
		string[] array = ServiceManager.Instance.LastAvgWaitTime.Split(' ');
		float result = 0f;
		float.TryParse(array[0], out result);
		int num = (int)result;
		int result2 = 0;
		int.TryParse(array[1], out result2);
		int num2 = num / 60;
		int num3 = num % 60;
		if (averageWaitTimeText != null)
		{
			if (num3 < 10)
			{
				averageWaitTimeText.text = num2 + ":0" + num3;
			}
			else
			{
				averageWaitTimeText.text = num2 + ":" + num3;
			}
		}
		if (userNumberText != null)
		{
			userNumberText.text = string.Format("{0:#,0}", result2);
		}
	}

	private void OnAvgWaitTimeFailure()
	{
		if (!isLoadingLevel && averageWaitTimeText != null)
		{
			averageWaitTimeText.text = _matchingTimeUnavailable;
		}
	}

	private void OnMatchSuccess()
	{
		string[] array = ServiceManager.Instance.GetMatchGameName().Split(':');
		for (int i = 1; i < array.Length - 1; i++)
		{
			for (int j = 1; j < array.Length - 1; j++)
			{
				if (i != j && array[i] == array[j] && (array[i] == ServiceManager.Instance.GetStats().pid.ToString() || array[j] == ServiceManager.Instance.GetStats().pid.ToString()))
				{
					OnSessionError();
					return;
				}
			}
		}
		ServiceManager.Instance.IsPrivateMatch = false;
		if (this._matchmakingCompleted != null)
		{
			this._matchmakingCompleted();
		}
		StartCoroutine(backgroundLoadLevel(array[0]));
	}

	private void OnMatchFailure()
	{
		isMatchmaking = false;
		isLoadingLevel = false;
		switch (ServiceManager.Instance.LastError)
		{
		default:
		{
			int num;
			if (num == 1)
			{
				OnSessionError();
			}
			else
			{
				createErrorMessage(_matchingConnectionFailedTitle, _matchingConnectionFailedMessage, _matchingConnectionFailedCloseButton, string.Empty);
			}
			break;
		}
		case "Request cancelled":
			break;
		}
		StopAllCoroutines();
		StartCoroutine(switchToMenu("play_chooseGame"));
		loadCharacter(LoadoutManager.Instance.CurrentLoadout);
	}

	private void PreviousMenu()
	{
		if (!guiController.IsActive)
		{
			return;
		}
		lastMenu = previousMenuStack.Pop();
		if (string.IsNullOrEmpty(lastMenu))
		{
			EtceteraAndroidManager.alertButtonClickedEvent += AlertButtonClicked;
			EtceteraAndroid.showAlert(_androidQuitTitle, _androidQuitMessage, _androidQuitConfirm, _androidQuitDismiss);
			previousMenuStack.Push(string.Empty);
			return;
		}
		string[] array = lastMenu.Split('_');
		if (array.Length >= 2)
		{
			lastMenu = array[1] + "_" + array[0];
			if (array[0].Contains("main"))
			{
				previousMenuStack.Clear();
				previousMenuStack.Push(string.Empty);
			}
			androidBackButton.name = lastMenu;
			OnGUIButtonClicked(androidBackButton);
		}
		else
		{
			Debug.LogError("splitting the last menu anim resaulted in an unusable string");
		}
	}

	private void AlertButtonClicked(string button)
	{
		if (button == _androidQuitConfirm)
		{
			Application.Quit();
			Caching.CleanCache();
		}
	}

	private void OnDestroy()
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= AlertButtonClicked;
	}
}
