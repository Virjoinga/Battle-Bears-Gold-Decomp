using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Analytics;
using DeltaDNA;
using Prime31;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Bootloader : MonoBehaviour
{
	private struct AchievementEntry
	{
		public bool fullUnlock;

		public string achievement;

		public float percent;
	}

	private const string AF_DEV_KEY = "bWa5iHmxrSLR7mS2AdnHf4";

	private const string APP_BUNDLE_ID = "net.skyvu.battlebearsgold";

	private const string IOS_APP_ID = "625394271";

	private static Bootloader instance;

	private float _timeOfLastUnPause;

	private float _sessionTime;

	public bool _overrideQuality;

	public QualitySetting _forcedQuality = QualitySetting.ULTRA;

	public bool isIpad;

	public bool isIpadOne;

	public bool is4G;

	public bool isAndroid;

	public bool loadIntoWeaponTest;

	public string LEVEL_OVERRIDE = string.Empty;

	public bool permitItemModifiers = true;

	public string socialName;

	private IAchievement[] _achievements;

	private List<AchievementEntry> pendingAchievements = new List<AchievementEntry>();

	public float SessionTime
	{
		get
		{
			return _sessionTime + Time.realtimeSinceStartup - _timeOfLastUnPause;
		}
		private set
		{
			_sessionTime = value;
		}
	}

	public float TotalTimePlayed
	{
		get
		{
			float @float = PlayerPrefs.GetFloat("totalTimePlayed", 0f);
			return @float + Time.realtimeSinceStartup - _timeOfLastUnPause;
		}
		private set
		{
			PlayerPrefs.SetFloat("totalTimePlayed", value);
			PlayerPrefs.Save();
		}
	}

	public static Bootloader Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (Bootloader)Object.FindObjectOfType(typeof(Bootloader));
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	public bool InTutorial { get; set; }

	public string GetMD5Hash(string input)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.UTF8.GetBytes(input);
		bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = bytes;
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("x2").ToLower());
		}
		return stringBuilder.ToString();
	}

	private void Awake()
	{
		Application.targetFrameRate = 30;
		PhysicsCollisionMatrixLayerMasks.Init();
		InitializeAppsFlyer();
		Singleton<DDNA>.Instance.Init();
		if (_overrideQuality)
		{
			BBRQuality.Current = _forcedQuality;
		}
		else if (!PlayerPrefs.HasKey("quality"))
		{
			BBRQuality.AutoDetectQuality();
		}
		else
		{
			BBRQuality.Current = (QualitySetting)PlayerPrefs.GetInt("quality");
		}
		isAndroid = Application.platform == RuntimePlatform.Android;
	}

	private void InitializeAppsFlyer()
	{
		AppsFlyer.setAppsFlyerKey("bWa5iHmxrSLR7mS2AdnHf4");
		AppsFlyer.setAppID("net.skyvu.battlebearsgold");
		AppsFlyer.init("bWa5iHmxrSLR7mS2AdnHf4", "AppsFlyerTrackerCallbacks");
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (PlayerPrefs.HasKey("hasShownChartboost"))
		{
			PlayerPrefs.DeleteKey("hasShownChartboost");
		}
		MogaController mogaController = MogaController.Instance;
		FacebookAndroid.init();
		GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlTbZhxfU5IGp3+uNTxBvsY3QeSXbs+4PRNEQZrAAWhAQHuwTIRRmLVEtILn16HiPAhn/3X0UiJyG47JA/vzevH5uZlGNDKO5HsDDoANGWjvvhou5tVs0EThM/AYbkTkLq2GpOVg4ZxiOPAJR4dS4PX784trhVA6D0RdZFRpoqdO8Ie2Geua7Aee227D7bwB7XU7UWnrmjyBC6xmPTmGPP5+07aBBywEj79zRlTijtpTl6hw9JhEYlBFPhcXwMrzasDXBAJmBSpAX5KY0EQBzlHCz2VSBhU2gv/gu90LlJexPX//xFCidEa1jLFqKUCGkrbSbKAo9RoC36yzjbeo8ZwIDAQAB");
		Screen.sleepTimeout = -1;
		ServiceManager.Instance.RefreshServers(OnGetServerSuccess, OnGetServerFailure);
		StartCoroutine(handlePendingAchievements());
	}

	private void OnGetServerFailure()
	{
		LoginManager.offlineMode = true;
		Application.LoadLevel("Login");
	}

	private void OnGetServerSuccess()
	{
		string @string = PlayerPrefs.GetString("previous_motd", string.Empty);
		string val = string.Empty;
		ServiceManager.Instance.UpdateProperty("message_of_the_day", ref val);
		if (val != @string && val != string.Empty)
		{
			PlayerPrefs.SetString("previous_motd", val);
			EtceteraAndroid.showAlert("Message of the day", val, "OK");
		}
		ServiceManager.Instance.LogGameLeft("game_crashed");
		ServiceManager.Instance.UpdateForceMatchingServerForPlatform("android_eight_person");
		if (loadIntoWeaponTest)
		{
			Application.LoadLevel("WeaponTest");
		}
		else
		{
			Application.LoadLevel("Login");
		}
	}

	public void unlockAchievement(string achievement)
	{
		AchievementEntry item = default(AchievementEntry);
		item.fullUnlock = true;
		item.achievement = achievement;
		item.percent = 100f;
		pendingAchievements.Add(item);
	}

	public void unlockAchievement(string achievement, float percent)
	{
		AchievementEntry item = default(AchievementEntry);
		item.fullUnlock = percent >= 100f;
		item.achievement = achievement;
		item.percent = percent;
		pendingAchievements.Add(item);
	}

	public void reportScore(string leaderboard, double score)
	{
	}

	private IEnumerator handlePendingAchievements()
	{
		while (true)
		{
			if (pendingAchievements.Count > 0)
			{
				pendingAchievements.RemoveAt(0);
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private void OnApplicationQuit()
	{
		EventTracker.TrackLastEvent();
		TrackTimePlayed();
		Caching.ClearCache();
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (isPaused)
		{
			TrackTimePlayed();
			EventTracker.TrackLastEvent();
		}
		else
		{
			_timeOfLastUnPause = Time.realtimeSinceStartup;
		}
	}

	private void TrackTimePlayed()
	{
		float num = Time.realtimeSinceStartup - _timeOfLastUnPause;
		_sessionTime += num;
		TotalTimePlayed = TotalTimePlayed;
		_timeOfLastUnPause = Time.realtimeSinceStartup;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
		}
	}

	private void MemoryWarning()
	{
		PlayerPrefs.DeleteKey("quality");
		PlayerPrefs.Save();
	}
}
