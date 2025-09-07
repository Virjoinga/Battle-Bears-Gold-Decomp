using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
	private static AdManager _instance;

	private bool _didInitialize;

	private bool _shownGameLaunchAd;

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
	}

	private void HandleSuccessfulInitialization()
	{
		Debug.Log("Tapjoy Initialized successfully.");
		_haveNotReceivedInitCallback = false;
		_didInitialize = true;
		FetchAd(AdType.mainMenu);
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
		
	}

	public void ShowAd(AdType adType)
	{
		
	}
}
