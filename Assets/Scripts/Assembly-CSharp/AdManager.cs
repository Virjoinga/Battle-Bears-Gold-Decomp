using System.Collections.Generic;
using TapjoyUnity;
using UnityEngine;

public class AdManager : MonoBehaviour
{
	private static AdManager _instance;

	private bool _didInitialize;

	private bool _shownGameLaunchAd;

	private Dictionary<AdType, TapjoyAd> _activeAds = new Dictionary<AdType, TapjoyAd>();

	private bool _haveNotReceivedInitCallback = true;

	private string _sessionId;

	public MainMenu Menu;

	public static AdManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool DidInitialize
	{
		get
		{
			return _didInitialize;
		}
	}

	public bool HaveNotReceivedInitCallback
	{
		get
		{
			return _haveNotReceivedInitCallback;
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		ServiceManager.Instance.SessionIdUpdated += SetSessionIdAsUserId;
		SetupTapjoyConnection();
		TJPlacement.OnVideoStart += PauseMusic;
		TJPlacement.OnVideoComplete += ResumeMusic;
	}

	private void SetupTapjoyConnection()
	{
		if (!Tapjoy.IsConnected)
		{
			Tapjoy.OnConnectSuccess += HandleSuccessfulInitialization;
			Tapjoy.OnConnectFailure += HandleFailedInitialization;
			Tapjoy.Connect("FqelhaTOR7CVs8vX06X59AECtlk9tvOluOlzFweD9YRKkqJsaQAHK9L58OIU");
		}
		else
		{
			HandleSuccessfulInitialization();
		}
	}

	private void HandleSuccessfulInitialization()
	{
		Debug.Log("Tapjoy Initialized successfully.");
		_haveNotReceivedInitCallback = false;
		_didInitialize = true;
		FetchAd(AdType.mainMenu);
	}

	private void PauseMusic(TJPlacement placement)
	{
		SoundManager.Instance.pauseMusic();
	}

	private void ResumeMusic(TJPlacement placement)
	{
		SoundManager.Instance.resumeMusic();
	}

	private void HandleFailedInitialization()
	{
		Debug.Log("Tapjoy Initialization failed.");
		_haveNotReceivedInitCallback = false;
		_didInitialize = false;
	}

	private void SetSessionIdAsUserId(string sessionId)
	{
		_sessionId = sessionId;
		if (_didInitialize)
		{
			SetUserIdOnTapjoyConnect();
		}
		else
		{
			Tapjoy.OnConnectSuccess += SetUserIdOnTapjoyConnect;
		}
	}

	private void SetUserIdOnTapjoyConnect()
	{
		Tapjoy.OnConnectSuccess -= SetUserIdOnTapjoyConnect;
		Tapjoy.SetUserID(_sessionId);
	}

	public void ShowFeaturedOffer()
	{
		if (_didInitialize && !_shownGameLaunchAd)
		{
			_shownGameLaunchAd = true;
			ShowAd(AdType.mainMenu);
		}
	}

	public void FetchAd(AdType adType, bool showWhenFetched = false)
	{
		if (_activeAds.ContainsKey(adType))
		{
			_activeAds[adType].Fetch(showWhenFetched);
			return;
		}
		TapjoyAd tapjoyAd = new TapjoyAd(adType, this);
		tapjoyAd.PlacementCompleted += HandlePlacementCompleted;
		_activeAds.Add(adType, tapjoyAd);
		tapjoyAd.Fetch(showWhenFetched);
	}

	private void HandlePlacementCompleted(TapjoyAd ad)
	{
		if (Menu != null)
		{
			Menu.OnUpdatePlayerStats();
		}
		if (_activeAds.ContainsKey(ad.Type))
		{
			_activeAds.Remove(ad.Type);
		}
	}

	public void ShowAd(AdType adType)
	{
		if (_activeAds.ContainsKey(adType))
		{
			_activeAds[adType].Show();
			return;
		}
		TapjoyAd tapjoyAd = new TapjoyAd(adType, this);
		tapjoyAd.PlacementCompleted += HandlePlacementCompleted;
		_activeAds.Add(adType, tapjoyAd);
		tapjoyAd.Show();
	}
}
