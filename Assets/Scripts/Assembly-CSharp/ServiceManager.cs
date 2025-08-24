using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using LitJson;
using Prime31;
using UnityEngine;

public class ServiceManager : MonoBehaviour, ServiceInterface
{
	private class Response
	{
		public string error;

		public string session;

		public bool rep_reward;

		public Dictionary<string, string> versions;

		public List<int> locker;

		public Stats stats;

		public List<Item> inventory;

		public List<Server> servers;

		public List<string> tips;

		public List<Purchaseable> store_items;

		public List<CrossPromotionItem> cross_promotion_items;

		public Dictionary<string, int> itunes_items;

		public Dictionary<string, int> google_play_items;

		public Dictionary<string, int> microsoft_store_items;

		public Dictionary<string, int> trade_items;

		public Dictionary<string, string> settings;

		public Dictionary<string, Reward> rewards;

		public Dictionary<string, int> player_rewards;

		public Dictionary<string, string> reputation;

		public List<Deal> deals;

		public int daily_reward = -1;

		public static Response Test()
		{
			Response response = new Response();
			response.error = "Something bad happened";
			response.session = "12345";
			response.locker = new List<int>(new int[3] { 5, 1, 2 });
			response.stats = Stats.Test();
			response.inventory = new List<Item>();
			response.inventory.Add(Item.Test());
			response.servers = new List<Server>();
			response.servers.Add(Server.Test());
			response.store_items = new List<Purchaseable>();
			response.reputation = new Dictionary<string, string>();
			return response;
		}
	}

	private const string ROYALE_SERVER = "royale";

	private const string ROYALE_MAP = "CastleRoyale";

	public const string CLOUD_PHOTON_SERVER = "app-us.exitgamescloud.com";

	private const string WINDOWS_PLATFORM = "windows";

	private const string ANDROID_PLATFORM = "android";

	private const string IOS_PLATFORM = "ios";

	private WWW matchmakingRequest;

	private WWW reportSubmitRequest;

	private uint sessionRequestSecret;

	private int matchRequestIndex;

	private float matchStartTime;

	private int numberOfUsers;

	private bool _gotRepReward;

	private string _lastProductID;

	private FinishedCallback purchaseSuccess;

	private FinishedCallback purchaseFailed;

	private FinishedCallback purchaseCancelled;

	public ServerAddressType serverAddress;

	public MatchingServerType matchingServer;

	public string CLIENT_VERSION_STRING = string.Empty;

	private string INVENTORY_VERSION_STRING = string.Empty;

	private string STORE_VERSION_STRING = string.Empty;

	private string ROOT_SERVER_URL = "http://192.168.1.138:3000";

	private string ROOT_SERVER_IP_ADDRESS;

	private string FORCE_PHOTON_SERVER = string.Empty;

	private string FORCE_MATCH_SERVER = "107.21.119.208:3008";

	private static ServiceManager instance;

	private string _privateMatchingPlatformsSuffix = "_private_matching_platforms";

	private string _privateMatchPlatforms = string.Empty;

	private string _nameColor = "white";

	private string lastError;

	private Dictionary<string, List<Server>> servers = new Dictionary<string, List<Server>>();

	private List<string> tips = new List<string>();

	private Stats stats = new Stats();

	private Dictionary<string, Item> items_by_name = new Dictionary<string, Item>();

	private Dictionary<int, Item> items_by_id = new Dictionary<int, Item>();

	private Dictionary<int, Purchaseable> purchaseables = new Dictionary<int, Purchaseable>();

	private Dictionary<int, Item> locker = new Dictionary<int, Item>();

	private Dictionary<string, string> properties = new Dictionary<string, string>();

	private Dictionary<string, int> iTunesProductIDs = new Dictionary<string, int>();

	private Dictionary<string, int> googlePlayProductIDs = new Dictionary<string, int>();

	private Dictionary<string, int> microsoftStoreProductIds = new Dictionary<string, int>();

	private Dictionary<int, int> jouleExchangeRates = new Dictionary<int, int>();

	private Dictionary<string, Reward> rewards = new Dictionary<string, Reward>();

	private Dictionary<string, int> player_rewards = new Dictionary<string, int>();

	private List<Deal> deals = new List<Deal>();

	private List<CrossPromotionItem> crossPromotionItems = new List<CrossPromotionItem>();

	private int daily_reward = -1;

	private string photonServerURL;

	private string photonServerGameName;

	private string lastMatchServerURL;

	private bool waitingForResults;

	private string avgWaitTime;

	private Report.GameReport lastGameReport;

	private GameResults lastGameResults = new GameResults();

	private Dictionary<string, double> propertyMaximums = new Dictionary<string, double>();

	public bool GotRepReward
	{
		get
		{
			bool gotRepReward = _gotRepReward;
			_gotRepReward = false;
			return gotRepReward;
		}
		private set
		{
			_gotRepReward = value;
		}
	}

	public static ServiceInterface Instance
	{
		get
		{
			return instance;
		}
	}

	public string LastError
	{
		get
		{
			return lastError;
		}
	}

	public bool WaitingForResults
	{
		get
		{
			return waitingForResults;
		}
	}

	public Report.GameReport LastGameReport
	{
		get
		{
			return lastGameReport;
		}
	}

	public GameResults LastGameResults
	{
		get
		{
			return lastGameResults;
		}
	}

	public string LastAvgWaitTime
	{
		get
		{
			return avgWaitTime;
		}
	}

	public string BuildIdentifier
	{
		get
		{
			return CLIENT_VERSION_STRING + " " + ROOT_SERVER_URL;
		}
	}

	public string ClientVersion
	{
		get
		{
			return CLIENT_VERSION_STRING;
		}
	}

	public string Platform { get; protected set; }

	public string PrivateMatchPlatforms
	{
		get
		{
			if (_privateMatchPlatforms.Equals(string.Empty) && properties.ContainsKey(Platform + _privateMatchingPlatformsSuffix))
			{
				_privateMatchPlatforms = properties[Platform + _privateMatchingPlatformsSuffix];
			}
			return _privateMatchPlatforms;
		}
	}

	public string NameColor
	{
		get
		{
			return _nameColor;
		}
	}

	public bool IsPrivateMatch { get; set; }

	private bool requestInProgress { get; set; }

	public string SessionId { get; private set; }

	private event Action<string> _sessionIdUpdated;

	public event Action<string> SessionIdUpdated
	{
		add
		{
			this._sessionIdUpdated = (Action<string>)Delegate.Combine(this._sessionIdUpdated, value);
		}
		remove
		{
			this._sessionIdUpdated = (Action<string>)Delegate.Remove(this._sessionIdUpdated, value);
		}
	}

	public void CheckMaliciousPlayer(Action<bool> callback)
	{
		StartCoroutine(CheckMaliciousPlayerRoutine(callback));
	}

	private IEnumerator CheckMaliciousPlayerRoutine(Action<bool> callback)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/reputations/" + SessionId + ".json");
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(CheckMaliciousPlayerRoutine(callback));
		}
		else
		{
			callback(www.text.Contains("true"));
		}
	}

	public int GetDailyReward()
	{
		return daily_reward;
	}

	public List<Deal> GetDeals()
	{
		return deals;
	}

	public Deal GetDeal(int dealID)
	{
		foreach (Deal deal in deals)
		{
			if (deal.id == dealID)
			{
				return deal;
			}
		}
		return null;
	}

	public Reward GetReward(string rewardName)
	{
		if (rewards.ContainsKey(rewardName))
		{
			return rewards[rewardName];
		}
		return null;
	}

	public bool PlayerHasReward(string rewardName)
	{
		return player_rewards.ContainsKey(rewardName);
	}

	public int GetPlayerReward(string rewardName)
	{
		if (player_rewards.ContainsKey(rewardName))
		{
			return player_rewards[rewardName];
		}
		return -1;
	}

	public float getPropertyMax(string propertyName)
	{
		if (propertyMaximums.ContainsKey(propertyName))
		{
			return (float)propertyMaximums[propertyName];
		}
		return 0f;
	}

	public int GetCurrentMatchServerIndex()
	{
		return matchRequestIndex;
	}

	public static Rank GetRank(double skill)
	{
		int val = 0;
		int val2 = 0;
		int val3 = 0;
		int val4 = 0;
		Instance.UpdateProperty("rank_bronze", ref val);
		Instance.UpdateProperty("rank_silver", ref val2);
		Instance.UpdateProperty("rank_gold", ref val3);
		Instance.UpdateProperty("rank_diamond", ref val4);
		if (skill < (double)val)
		{
			return Rank.green;
		}
		if (skill < (double)val2)
		{
			return Rank.bronze;
		}
		if (skill < (double)val3)
		{
			return Rank.silver;
		}
		if (skill < (double)val4)
		{
			return Rank.gold;
		}
		return Rank.diamond;
	}

	public void CreateAccount(string user, string pass, bool isGuest, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(CreateAccountCoroutine(user, pass, isGuest, success, failure));
	}

	public void RequestPasswordReset(string user, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RequestPasswordResetCoroutine(user, success, failure));
	}

	public void RequestEmailResend(string user, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RequestEmailResendCoroutine(user, success, failure));
	}

	public void Login(string user, string pass, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(LoginCoroutine(user, pass, success, failure));
	}

	public void UpgradeFromGuest(string user, string pass, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(UpgradeFromGuestCoroutine(user, pass, success, failure));
	}

	public void RefreshItems(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RefreshItemsCoroutine(success, failure));
	}

	public void RefreshPlayerLocker(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RefreshPlayerLockerCoroutine(success, failure));
	}

	public void RefreshPlayerStats(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RefreshPlayerStatsCoroutine(success, failure));
	}

	public void RefreshStoreItemList(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RefreshStoreItems(success, failure));
	}

	public void RefreshServers(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RefreshServersCoroutine(success, failure));
	}

	public void PurchaseItem(Purchaseable pItem, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(PurchaseItemCoroutine(pItem.id, success, failure));
	}

	public void PurchaseDeal(int dealID, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(PurchaseDealCoroutine(dealID, success, failure));
	}

	public void RequestReward(string rewardName, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RequestRewardCoroutine(rewardName, success, failure));
	}

	public void RefreshITunesProductList(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(GetITunesProductListCoroutine(success, failure));
	}

	public void RefreshGooglePlayProductList(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(GetGooglePlayProductListCoroutine(success, failure));
	}

	public void RefreshMicrosoftStoreProductList(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(GetMicrosoftStoreProductListCoroutine(success, failure));
	}

	public void PurchaseCurrency(string productID, int quantity, FinishedCallback success, FinishedCallback failure, FinishedCallback cancel)
	{
		purchaseFailed = failure;
		purchaseSuccess = success;
		purchaseCancelled = cancel;
		_lastProductID = productID;
		EventTracker.TrackEvent(IAPTransactionEventHelper.PurchaseStarted(productID));
		GoogleIAB.purchaseProduct(productID);
	}

	public void ValidateGasPurchase(string id, string receipt, int quantity, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(ValidateItunesGasPurchaseCoroutine(id, receipt, quantity, success, failure));
	}

	public void ValidateGoogleGasPurchase(string signedData, string signature, int quantity, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(ValidateGoogleGasPurchaseCoroutine(_lastProductID, signedData, signature, quantity, success, failure));
	}

	public void ValidateGoogleGasPurchase(string signedData, string signature, int quantity)
	{
		StartCoroutine(ValidateGoogleGasPurchaseCoroutine(_lastProductID, signedData, signature, quantity, purchaseSuccess, purchaseFailed));
	}

	public void ValidateMicrosoftStorePurchase(string receipt, int quantity, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(ValidateMicrosoftStorePurchaseCoroutine(receipt, quantity, success, failure));
	}

	public void TradeGasForJoules(int cansPerSet, int nSets, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(TradeGasForJoulesCoroutine(cansPerSet, nSets, success, failure));
	}

	public void OfferwallTransaction(int amtOfGas, string receiptID, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(OfferwallTransactionCoroutine(amtOfGas, receiptID, success, failure));
	}

	public void RefillEnergy(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RefillEnergyCoroutine(success, failure));
	}

	public void SendReport(Report.GameReport report, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(SendReportCoroutine(report, success, failure));
	}

	public void RequestWaitTime(int index, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(RequestAverageWaitTimeCoroutine(index, success, failure));
	}

	public void RequestGame(int index, FinishedCallback success, FinishedCallback failure)
	{
		matchRequestIndex = index;
		StartCoroutine(RequestGameCoroutine(index, success, failure));
	}

	public void NotifyGameStart(FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(NotifyGameStartCoroutine(success, failure));
	}

	public void ReportPlayer(int playerToBeReported, int reportType, FinishedCallback success, FinishedCallback failure)
	{
		StartCoroutine(ReportPlayerCoroutine(playerToBeReported, reportType, success, failure));
	}

	public void LogGameLeft(string reason)
	{
		string @string = PlayerPrefs.GetString("last_joined_game");
		if (@string != string.Empty)
		{
			PlayerPrefs.SetString("last_joined_game", string.Empty);
		}
	}

	public void NotifyPurchaseFailed(string error)
	{
		Debug.LogError("Purchase failed " + error);
		lastError = error;
		EventTracker.TrackEvent(IAPTransactionEventHelper.PurchaseFailed(_lastProductID, error));
		if (purchaseFailed != null)
		{
			purchaseFailed();
		}
	}

	public void NotifyPurchaseCancelled(string error)
	{
		lastError = error;
		EventTracker.TrackEvent(IAPTransactionEventHelper.PurchaseFailed(_lastProductID, error));
		if (purchaseCancelled != null)
		{
			purchaseCancelled();
		}
	}

	public void NotifyPurchaseSuccess(string id, string receipt, int quantity)
	{
		ValidateGasPurchase(id, receipt, quantity, purchaseSuccess, purchaseFailed);
	}

	public string GetServerURL()
	{
		return ROOT_SERVER_URL;
	}

	public string GetInventoryVersion()
	{
		return INVENTORY_VERSION_STRING;
	}

	private void Awake()
	{
		instance = this;
		switch (serverAddress)
		{
		case ServerAddressType.DEVELOPMENT:
		{
			ROOT_SERVER_IP_ADDRESS = "http://107.21.119.208/dev/store";
			ROOT_SERVER_URL = "http://107.21.119.208/dev/store";
			if (matchingServer == MatchingServerType.NONE)
			{
				FORCE_MATCH_SERVER = string.Empty;
				break;
			}
			int num3 = (int)matchingServer;
			FORCE_MATCH_SERVER = "http://107.21.119.208:700" + num3;
			break;
		}
		case ServerAddressType.STAGING:
		{
			ROOT_SERVER_IP_ADDRESS = "http://107.21.119.208/staging/store";
			ROOT_SERVER_URL = "http://107.21.119.208/staging/store";
			if (matchingServer == MatchingServerType.NONE)
			{
				FORCE_MATCH_SERVER = string.Empty;
				break;
			}
			int num2 = (int)matchingServer;
			FORCE_MATCH_SERVER = "http://107.21.119.208:300" + num2;
			break;
		}
		case ServerAddressType.PRODUCTION:
		{
			ROOT_SERVER_IP_ADDRESS = "http://50.17.197.137/1.2/store";
			ROOT_SERVER_URL = "http://bbr2.battlebears.com/1.2/store";
			if (matchingServer == MatchingServerType.NONE)
			{
				FORCE_MATCH_SERVER = string.Empty;
				break;
			}
			int num = (int)matchingServer;
			FORCE_MATCH_SERVER = "http://107.21.119.208:300" + num;
			break;
		}
		default:
			ROOT_SERVER_IP_ADDRESS = "http://50.17.197.137/1.2/store";
			ROOT_SERVER_URL = "http://bbr2.battlebears.com/1.2/store";
			FORCE_MATCH_SERVER = string.Empty;
			break;
		}
		if (requestInProgress)
		{
			LogConnectionFailure("crash");
			requestInProgress = false;
		}
		string @string = PlayerPrefs.GetString("cached_store", string.Empty);
		if (@string != string.Empty)
		{
			STORE_VERSION_STRING = Bootloader.Instance.GetMD5Hash(@string);
			try
			{
				UpdatePurchaseables(JsonMapper.ToObject<List<Purchaseable>>(@string));
			}
			catch (Exception ex)
			{
				Debug.LogWarning("using incompatible store in cache, skipping preloading: " + ex.ToString());
			}
		}
		string string2 = PlayerPrefs.GetString("cached_inventory", string.Empty);
		if (string2 != string.Empty)
		{
			INVENTORY_VERSION_STRING = Bootloader.Instance.GetMD5Hash(string2);
			try
			{
				UpdateInventory(JsonMapper.ToObject<List<Item>>(string2));
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("have incompatible inventory in cache, skipping preloading: " + ex2.ToString());
			}
		}
	}

	public void Start()
	{
		Platform = "android";
	}

	public void UpdateForceMatchingServerForPlatform(string platform)
	{
		if (!string.IsNullOrEmpty(FORCE_MATCH_SERVER))
		{
			return;
		}
		List<Server> list = GetServers("match");
		foreach (Server item in list)
		{
			if (item.description.Contains(platform))
			{
				FORCE_MATCH_SERVER = item.url;
			}
		}
	}

	public List<Server> GetServers(string type)
	{
		if (!servers.ContainsKey(type))
		{
			return null;
		}
		return servers[type];
	}

	public List<string> GetTips()
	{
		return tips;
	}

	public Item GetItemByName(string name)
	{
		if (name != null && name != string.Empty)
		{
			if (items_by_name.ContainsKey(name))
			{
				return items_by_name[name];
			}
			Debug.LogWarning("Item " + name + " not found!");
		}
		return new Item();
	}

	public Item GetItemByID(int id)
	{
		if (items_by_id.ContainsKey(id))
		{
			return items_by_id[id];
		}
		return new Item();
	}

	public IList<Item> GetItemsForCharacterAndType(string character, string type)
	{
		int id = GetItemByName(character).id;
		List<Item> list = new List<Item>();
		foreach (Item value in items_by_id.Values)
		{
			if (value.parent_id == id && value.type == type)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public IList<Item> GetItemsForType(string type)
	{
		List<Item> list = new List<Item>();
		foreach (Item value in items_by_id.Values)
		{
			if (value.type == type)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public Item GetItemByName(string name, string character, ref bool hadReplace)
	{
		if (name != null && name != string.Empty)
		{
			if (items_by_name.ContainsKey(name))
			{
				return items_by_name[name];
			}
			hadReplace = true;
			int parent_id = GetItemByName(character).parent_id;
			Item result = new Item();
			{
				foreach (Item value in items_by_id.Values)
				{
					if (value.parent_id == parent_id)
					{
						result = value;
						if (value.is_default)
						{
							return value;
						}
					}
				}
				return result;
			}
		}
		return new Item();
	}

	public Item GetDefaultReplacement(Item itemToReplace)
	{
		if (itemToReplace.type == "character")
		{
			return GetItemByName("Oliver");
		}
		Item result = new Item();
		foreach (Item value in items_by_id.Values)
		{
			if (value.parent_id == itemToReplace.parent_id)
			{
				result = value;
				if (value.is_default)
				{
					return value;
				}
			}
		}
		return result;
	}

	public Purchaseable GetPurchaseableByID(int id)
	{
		if (purchaseables.ContainsKey(id))
		{
			return purchaseables[id];
		}
		return new Purchaseable();
	}

	public bool IsItemBought(int item_id)
	{
		return locker.ContainsKey(item_id);
	}

	public Dictionary<int, Item> GetLocker()
	{
		return locker;
	}

	public Stats GetStats()
	{
		return stats;
	}

	public Dictionary<string, Item> GetAllItemsByName()
	{
		return new Dictionary<string, Item>(items_by_name);
	}

	public Dictionary<int, Purchaseable> GetStore()
	{
		return purchaseables;
	}

	private string GetMatchmakingServerRootUrl(int index)
	{
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			return GetServers("royale")[index].url;
		}
		return (!string.IsNullOrEmpty(FORCE_MATCH_SERVER)) ? FORCE_MATCH_SERVER : GetServers("match")[index].url;
	}

	public string GetLastMatchServerUsed()
	{
		return lastMatchServerURL;
	}

	public string GetMatchGameServer()
	{
		return "app-us.exitgamescloud.com";
	}

	public string GetPhotonAppID()
	{
		if (properties.ContainsKey("photon_app_id"))
		{
			return properties["photon_app_id"];
		}
		return null;
	}

	public string GetMatchGameName()
	{
		return photonServerGameName;
	}

	public void SetMatchGameName(string name)
	{
		photonServerGameName = name;
	}

	public string GetRuntimeString()
	{
		return "android";
	}

	private IEnumerator LoginCoroutine(string user, string password, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("username", user);
		form.AddField("password", password);
		form.AddField("utc_offset", TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString());
		form.AddField("client_version", CLIENT_VERSION_STRING);
		form.AddField("runtime", GetRuntimeString());
		form.AddField("rep", 1);
		WWW www = new WWW(ROOT_SERVER_URL + "/sign_in.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(LoginCoroutine(user, password, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator UpgradeFromGuestCoroutine(string user, string password, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("username", user);
		form.AddField("password", password);
		form.AddField("session", SessionId);
		WWW www = new WWW(ROOT_SERVER_URL + "/registrations.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(UpgradeFromGuestCoroutine(user, password, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator CreateAccountCoroutine(string user, string password, bool isGuest, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("username", user);
		form.AddField("password", password);
		form.AddField("guest", (!isGuest) ? "0" : "1");
		form.AddField("client_version", CLIENT_VERSION_STRING);
		WWW www = new WWW(ROOT_SERVER_URL + "/players.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(CreateAccountCoroutine(user, password, isGuest, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RequestPasswordResetCoroutine(string user, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("email", user);
		WWW www = new WWW(ROOT_SERVER_URL + "/passwords.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RequestPasswordResetCoroutine(user, success, failure));
		}
		else if (WWWErrorCheck(www) && failure != null)
		{
			lastError = www.error;
			failure();
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RequestEmailResendCoroutine(string user, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("email", user);
		WWW www = new WWW(ROOT_SERVER_URL + "/activations.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RequestEmailResendCoroutine(user, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator ReportPlayerCoroutine(int playerToBeReported, int reportType, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("player", playerToBeReported);
		form.AddField("type", reportType);
		form.AddField("session", SessionId);
		WWW www = new WWW(ROOT_SERVER_URL + "/behavior_reports.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(ReportPlayerCoroutine(playerToBeReported, reportType, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RequestAverageWaitTimeCoroutine(int index, FinishedCallback success, FinishedCallback failure)
	{
		string serverString = GetMatchmakingServerRootUrl(index) + "/?request=waittime&" + AddTimeString();
		WWW www = new WWW(serverString);
		yield return www;
		if (WWWErrorCheck(www))
		{
			Debug.LogError("Request wait time failed : " + www.error);
			lastError = www.error;
			if (failure != null)
			{
				failure();
			}
			yield break;
		}
		string statsString3 = www.text;
		statsString3 = statsString3.Substring(statsString3.IndexOf('\n'));
		statsString3 = statsString3.Trim();
		string[] statsSplit = statsString3.Split('\n');
		string[] statsParts = statsSplit[0].Split(' ');
		if (statsParts[0] != "STATS:")
		{
			Debug.LogError("Result was not a stats string");
			lastError = "Result was not a stats string";
			if (failure != null)
			{
				failure();
			}
		}
		else
		{
			avgWaitTime = statsParts[1] + " " + statsParts[2];
			success();
		}
	}

	private IEnumerator RequestGameCoroutine(int index, FinishedCallback success, FinishedCallback failure)
	{
		if (requestInProgress)
		{
			Debug.LogWarning("Request already in progress, bailing out");
			lastError = "Request in progress";
			if (failure != null)
			{
				failure();
			}
			yield break;
		}
		requestInProgress = true;
		yield return StartCoroutine(RefreshPlayerStatsCoroutine(null, failure));
		if (lastError != null)
		{
			requestInProgress = false;
			yield break;
		}
		sessionRequestSecret = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		bool matchByLevel = false;
		UpdateProperty("use_level_for_matching", ref matchByLevel);
		double skill = stats.skill;
		if (matchByLevel)
		{
			float maxLevel = 60f;
			float maxSkill = 9000f;
			skill = (int)(stats.level / (double)maxLevel * (double)maxSkill);
		}
		string serverString2 = GetMatchmakingServerRootUrl(index);
		string text = serverString2;
		serverString2 = (lastMatchServerURL = text + "/?request=match&uid=" + stats.pid + "&secret=" + sessionRequestSecret + "&skill=" + skill + "&map=" + ((Preferences.Instance.CurrentGameMode != GameMode.ROYL) ? "*" : "CastleRoyale") + "&mode=" + Preferences.Instance.CurrentGameModeStr + "&session=" + SessionId + "&" + AddTimeString());
		matchRequestIndex = index;
		int spliceIndex = lastMatchServerURL.IndexOf('/', lastMatchServerURL.IndexOf("://") + 3);
		if (spliceIndex >= 0)
		{
			lastMatchServerURL = lastMatchServerURL.Substring(0, spliceIndex);
		}
		matchStartTime = Time.realtimeSinceStartup;
		Debug.Log("Matchmaking server string: " + serverString2);
		matchmakingRequest = new WWW(serverString2);
		yield return matchmakingRequest;
		if (matchmakingRequest != null)
		{
			if (WWWErrorCheck(matchmakingRequest))
			{
				Debug.LogError("Request game failed : " + matchmakingRequest.error + " when talking to " + matchmakingRequest.url);
				LogConnectionFailure(matchmakingRequest.error);
				lastError = matchmakingRequest.error;
				matchmakingRequest.Dispose();
				matchmakingRequest = null;
				requestInProgress = false;
				Debug.LogError("Failed game request at index: " + index);
				int num;
				index = (num = index + 1);
				if (num < GetServers("match").Count)
				{
					Debug.LogError("Attemtping a new game request at index: " + index + " with " + GetServers("match").Count + " servers in our list");
					StartCoroutine(RequestGameCoroutine(index, success, failure));
				}
				else if (failure != null)
				{
					failure();
				}
			}
			else
			{
				string matchmakingString3 = matchmakingRequest.text;
				int firstCR = matchmakingString3.IndexOf('\n');
				if (firstCR == -1)
				{
					lastError = "Request game returned no match information";
					LogConnectionFailure("empty_response");
					matchmakingRequest.Dispose();
					matchmakingRequest = null;
					requestInProgress = false;
					if (failure != null)
					{
						failure();
					}
				}
				else
				{
					matchmakingString3 = matchmakingString3.Substring(firstCR);
					matchmakingString3 = matchmakingString3.Trim();
					string[] matchmakeParts = matchmakingString3.Split('\n');
					if (FORCE_PHOTON_SERVER != null && FORCE_PHOTON_SERVER != string.Empty)
					{
						photonServerURL = FORCE_PHOTON_SERVER;
					}
					else
					{
						photonServerURL = matchmakeParts[0].Trim();
					}
					SetMatchGameName(matchmakeParts[1].Trim());
					if (photonServerURL.StartsWith("ERROR"))
					{
						if (photonServerURL.EndsWith("Request cancelled"))
						{
							lastError = "Request cancelled";
							LogConnectionFailure("cancelled");
						}
						else
						{
							Debug.LogError("Request failed " + photonServerURL);
							lastError = photonServerURL;
							LogConnectionFailure("server_response_error");
						}
						photonServerURL = string.Empty;
						SetMatchGameName(string.Empty);
						matchmakingRequest.Dispose();
						matchmakingRequest = null;
						requestInProgress = false;
						if (failure != null)
						{
							failure();
						}
					}
					else
					{
						matchmakingRequest.Dispose();
						matchmakingRequest = null;
						int timeTakenToMatch = Mathf.RoundToInt(Time.realtimeSinceStartup - matchStartTime);
						EventTracker.TrackEvent(new MatchmakingCompletedSchema(new MatchmakingWaitTimeParameter(timeTakenToMatch)));
						PlayerPrefs.SetString("last_joined_game", GetMatchGameName());
						requestInProgress = false;
						if (success != null)
						{
							success();
						}
					}
				}
			}
		}
		requestInProgress = false;
	}

	private int GetPlayersInMatch()
	{
		try
		{
			return GetMatchGameName().Split(":".ToCharArray()).Length - 1;
		}
		catch (Exception)
		{
			return -1;
		}
	}

	private string GetMapForMatch()
	{
		try
		{
			return GetMatchGameName().Split(":".ToCharArray())[0];
		}
		catch (Exception)
		{
			return "unknown";
		}
	}

	private void LogConnectionFailure(string reason)
	{
		float num = Time.realtimeSinceStartup - matchStartTime;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string value = "long";
		if (num <= 5f)
		{
			value = "matching_time_5s";
		}
		else if (num <= 15f)
		{
			value = "matching_time_15s";
		}
		else if (num <= 30f)
		{
			value = "matching_time_30s";
		}
		else if (num <= 45f)
		{
			value = "matching_time_45s";
		}
		dictionary.Add("seconds", value);
		dictionary.Add("lobby_size", numberOfUsers);
		dictionary.Add("reason", reason);
		Debug.Log("Connection failure.  Matching left ");
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			Debug.Log("Key " + item.Key + " Value " + item.Value);
		}
	}

	public void CancelRequestGame()
	{
		int amount = Mathf.RoundToInt(Time.realtimeSinceStartup - matchStartTime);
		EventTracker.TrackEvent(new MatchmakingExitedSchema(new MatchmakingWaitTimeParameter(amount)));
		StartCoroutine(CancelRequestGameCoroutine());
	}

	private IEnumerator CancelRequestGameCoroutine()
	{
		if (matchmakingRequest != null)
		{
			string serverString = GetMatchmakingServerRootUrl(matchRequestIndex) + "/?request=abort&uid=" + stats.pid + "&secret=" + sessionRequestSecret + "&" + AddTimeString();
			WWW www = new WWW(serverString);
			yield return www;
			if (WWWErrorCheck(www))
			{
				Debug.LogError("Error : " + www.error);
			}
			if (matchmakingRequest != null)
			{
				matchmakingRequest.Dispose();
				matchmakingRequest = null;
			}
		}
	}

	private void OnDestroy()
	{
		Cleanup();
	}

	private void Cleanup()
	{
		if (matchmakingRequest != null)
		{
			matchmakingRequest.Dispose();
			matchmakingRequest = null;
		}
		if (reportSubmitRequest != null)
		{
			reportSubmitRequest.Dispose();
			reportSubmitRequest = null;
		}
	}

	private void OnApplicationQuit()
	{
		Cleanup();
		LogGameLeft("user_quit");
	}

	private string AddVersionStrings()
	{
		return "store_version=" + STORE_VERSION_STRING + "&inventory_version=" + INVENTORY_VERSION_STRING;
	}

	private string AddTimeString()
	{
		return "time=" + (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
	}

	private IEnumerator RefreshItemsCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/items.json?" + AddVersionStrings() + "&" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RefreshItemsCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RefreshPlayerStatsCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/stats.json?session=" + SessionId + "&" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RefreshPlayerStatsCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RefreshPlayerLockerCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/player.json?session=" + SessionId + "&" + AddVersionStrings() + "&runtime=" + GetRuntimeString() + "&" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RefreshPlayerLockerCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RefreshStoreItems(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/store_items.json?" + AddVersionStrings() + "&runtime=" + GetRuntimeString() + "&" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RefreshStoreItems(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RefreshServersCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/servers.json?" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RefreshServersCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator PurchaseItemCoroutine(int purchaseItemID, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("store_item_id", purchaseItemID);
		WWW www = new WWW(ROOT_SERVER_URL + "/transactions.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(PurchaseItemCoroutine(purchaseItemID, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator PurchaseDealCoroutine(int dealID, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("deal_id", dealID);
		WWW www = new WWW(ROOT_SERVER_URL + "/deal_transactions.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(PurchaseDealCoroutine(dealID, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RequestRewardCoroutine(string rewardName, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("name", rewardName);
		WWW www = new WWW(ROOT_SERVER_URL + "/rewards.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RequestRewardCoroutine(rewardName, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator GetITunesProductListCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/itunes_items.json");
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(GetITunesProductListCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator GetGooglePlayProductListCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/google_play_items.json");
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(GetGooglePlayProductListCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator GetMicrosoftStoreProductListCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/microsoft_store_items.json?" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(GetMicrosoftStoreProductListCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator ValidateItunesGasPurchaseCoroutine(string id, string receipt, int quantity, FinishedCallback success, FinishedCallback failure)
	{
		yield return null;
	}

	private IEnumerator ValidateGoogleGasPurchaseCoroutine(string productId, string signedData, string signature, int quantity, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("id", productId);
		form.AddField("qty", quantity);
		form.AddField("data", signedData);
		form.AddField("sig", signature);
		WWW www = new WWW(ROOT_SERVER_URL + "/google_play_transactions.json", form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(ValidateGoogleGasPurchaseCoroutine(productId, signedData, signature, quantity, success, failure));
			yield break;
		}
		EventTracker.TrackEvent(IAPTransactionEventHelper.VirtualCurrencyTransaction(productId, signedData, signature));
		string productId2 = default(string);
		string signedData2 = default(string);
		FinishedCallback success2 = default(FinishedCallback);
		string signature2 = default(string);
		FinishedCallback failure2 = default(FinishedCallback);
		ProcessJSONResponse(www, delegate
		{
			GoogleIAB.consumeProduct(productId2);
			EventTracker.TrackEvent(IAPTransactionEventHelper.PurchaseValidated(productId2, signedData2));
			success2();
		}, delegate
		{
			EventTracker.TrackEvent(IAPTransactionEventHelper.PurchaseInvalidated(productId2, signedData2, signature2));
			failure2();
		});
	}

	private IEnumerator ValidateMicrosoftStorePurchaseCoroutine(string receipt, int quantity, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("qty", quantity);
		form.AddField("receipt", receipt);
		WWW www = new WWW(ROOT_SERVER_URL + "/microsoft_store_transactions.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(ValidateMicrosoftStorePurchaseCoroutine(receipt, quantity, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private static byte[] GetBytes(string str)
	{
		byte[] array = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	public static string EncodeTo64(string toEncode)
	{
		return Convert.ToBase64String(GetBytes(toEncode));
	}

	private IEnumerator OfferwallTransactionCoroutine(int amtOfGas, string receiptID, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("gas", amtOfGas);
		form.AddField("receipt_id", receiptID);
		WWW www = new WWW(ROOT_SERVER_URL + "/offerwall_transactions.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(OfferwallTransactionCoroutine(amtOfGas, receiptID, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator TradeGasForJoulesCoroutine(int cansPerSet, int nSets, FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("gas", cansPerSet);
		form.AddField("qty", nSets);
		WWW www = new WWW(ROOT_SERVER_URL + "/trade_transactions.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(TradeGasForJoulesCoroutine(cansPerSet, nSets, success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator GetEnergyLevelCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/energy.json?session=" + SessionId + "&" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(GetEnergyLevelCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator RefillEnergyCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		WWW www = new WWW(ROOT_SERVER_URL + "/refill_energy.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(RefillEnergyCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator SendReportCoroutine(Report.GameReport report, FinishedCallback success, FinishedCallback failure)
	{
		if (Preferences.Instance.CurrentGameMode == GameMode.ROYL)
		{
			EventTracker.TrackEvent(MatchEventsHelper.MatchCompleted(report));
		}
		PlayerPrefs.SetString("last_joined_game", string.Empty);
		lastGameReport = report;
		waitingForResults = true;
		WWWForm form = new WWWForm();
		form.AddField("game_name", report.game_name);
		form.AddField("player_id", stats.pid);
		form.AddField("communication_type", "request");
		form.AddField("num_players", report.players.Count);
		form.AddField("report", report.GetJSON());
		string requestString = GetServers("report")[0].url + "/reportingscript.php?" + AddTimeString();
		reportSubmitRequest = new WWW(requestString, form);
		yield return reportSubmitRequest;
		if (WWWErrorCheck(reportSubmitRequest))
		{
			Debug.LogError("Report error : " + reportSubmitRequest.error);
		}
		ProcessReportJSONResponse(reportSubmitRequest, success, failure);
		reportSubmitRequest = null;
		waitingForResults = false;
	}

	private IEnumerator GetStatsCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		WWW www = new WWW(ROOT_SERVER_URL + "/stats.json?session=" + SessionId + "&" + AddTimeString());
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(GetStatsCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private IEnumerator NotifyGameStartCoroutine(FinishedCallback success, FinishedCallback failure)
	{
		EventTracker.TrackEvent(MatchEventsHelper.MatchStarted());
		WWWForm form = new WWWForm();
		form.AddField("session", SessionId);
		form.AddField("game_name", GetMatchGameName());
		form.AddField("server_name", GetMatchGameServer());
		WWW www = new WWW(ROOT_SERVER_URL + "/game_start.json?" + AddTimeString(), form);
		yield return www;
		if (WWWErrorCheck(www) && ROOT_SERVER_URL != ROOT_SERVER_IP_ADDRESS)
		{
			ROOT_SERVER_URL = ROOT_SERVER_IP_ADDRESS;
			StartCoroutine(NotifyGameStartCoroutine(success, failure));
		}
		else
		{
			ProcessJSONResponse(www, success, failure);
		}
	}

	private bool WWWErrorCheck(WWW www)
	{
		return www.error != null;
	}

	private void ProcessJSONResponse(WWW www, FinishedCallback success, FinishedCallback failure)
	{
		if (WWWErrorCheck(www))
		{
			Debug.LogError("WWW error " + www.error + " : " + www.url);
			lastError = www.error;
			if (failure != null)
			{
				failure();
			}
			return;
		}
		try
		{
			Response resp = JsonMapper.ToObject<Response>(www.text);
			ProcessJSONResponse(resp, success, failure);
		}
		catch (JsonException ex)
		{
			Debug.LogError(ex.ToString());
			lastError = "JsonException thrown";
			if (failure != null)
			{
				failure();
			}
		}
	}

	private void ProcessReportJSONResponse(WWW www, FinishedCallback success, FinishedCallback failure)
	{
		if (WWWErrorCheck(www))
		{
			Debug.LogError(www.error + " : " + www.url);
			lastError = www.error;
			lastGameResults = new GameResults();
			if (failure != null)
			{
				failure();
			}
			return;
		}
		try
		{
			lastGameResults = JsonMapper.ToObject<GameResults>(www.text);
			if (lastGameResults == null)
			{
				lastGameResults = new GameResults();
				lastError = "Got nothing back from reporting server";
				Debug.LogError(lastError);
				if (failure != null)
				{
					failure();
				}
			}
			else if (lastGameResults.errorString == null)
			{
				lastError = null;
				if (success != null)
				{
					success();
				}
			}
			else
			{
				lastError = lastGameResults.errorString;
				if (failure != null)
				{
					failure();
				}
			}
		}
		catch (JsonException ex)
		{
			Debug.LogError(ex.ToString());
			lastError = "JsonException thrown";
			if (failure != null)
			{
				failure();
			}
		}
	}

	private void ProcessJSONResponse(Response resp, FinishedCallback success, FinishedCallback failure)
	{
		if (resp == null)
		{
			if (failure != null)
			{
				failure();
			}
			return;
		}
		lastError = resp.error;
		if (resp.error == null)
		{
			if (resp.reputation != null)
			{
				UpdateNameColor(resp.reputation);
			}
			if (resp.locker != null)
			{
				UpdateLocker(resp.locker);
			}
			if (resp.versions != null)
			{
				UpdateVersions(resp.versions);
			}
			if (resp.servers != null)
			{
				UpdateServers(resp.servers);
			}
			if (resp.tips != null)
			{
				UpdateTips(resp.tips);
			}
			if (resp.rewards != null)
			{
				UpdateRewards(resp.rewards);
			}
			if (resp.player_rewards != null)
			{
				UpdatePlayerRewards(resp.player_rewards);
			}
			if (resp.cross_promotion_items != null)
			{
				UpdateCrossPromotionItems(resp.cross_promotion_items);
			}
			if (resp.rep_reward)
			{
				GotRepReward = true;
			}
			if (resp.deals != null)
			{
				UpdateDeals(resp.deals);
			}
			if (resp.inventory != null)
			{
				PlayerPrefs.SetString("cached_inventory", JsonMapper.ToJson(resp.inventory));
				UpdateInventory(resp.inventory);
			}
			if (resp.stats != null)
			{
				UpdateStats(resp.stats);
			}
			if (resp.daily_reward != -1)
			{
				UpdateDailyReward(resp.daily_reward);
			}
			if (resp.session != null)
			{
				UpdateSession(resp.session);
			}
			if (resp.store_items != null)
			{
				PlayerPrefs.SetString("cached_store", JsonMapper.ToJson(resp.store_items));
				UpdatePurchaseables(resp.store_items);
			}
			if (resp.settings != null)
			{
				UpdateProperties(resp.settings);
			}
			if (resp.trade_items != null)
			{
				UpdateJouleExchangeRates(resp.trade_items);
			}
			if (resp.itunes_items != null)
			{
				StartCoroutine(UpdateITunesProducts(resp.itunes_items, success, failure));
			}
			else if (resp.google_play_items != null)
			{
				StartCoroutine(UpdateGooglePlayProducts(resp.google_play_items, success, failure));
			}
			else if (resp.microsoft_store_items != null)
			{
				StartCoroutine(UpdateMicrosoftStoreProducts(resp.microsoft_store_items, success, failure));
			}
			else if (success != null)
			{
				success();
			}
		}
		else
		{
			Debug.LogError(resp.error);
			if (failure != null)
			{
				failure();
			}
		}
	}

	private void UpdateNameColor(Dictionary<string, string> reputation)
	{
		if (reputation.ContainsKey("color"))
		{
			_nameColor = reputation["color"];
		}
	}

	private void UpdateVersions(Dictionary<string, string> versions)
	{
		if (versions.ContainsKey("store"))
		{
			STORE_VERSION_STRING = versions["store"];
		}
		if (versions.ContainsKey("inventory"))
		{
			INVENTORY_VERSION_STRING = versions["inventory"];
		}
	}

	private void UpdateSession(string newSessionID)
	{
		SessionId = newSessionID;
		if (this._sessionIdUpdated != null)
		{
			this._sessionIdUpdated(SessionId);
		}
	}

	private void UpdateLocker(List<int> newLocker)
	{
		locker.Clear();
		foreach (int item in newLocker)
		{
			if (!locker.ContainsKey(item))
			{
				locker.Add(item, GetItemByID(item));
			}
		}
	}

	private void UpdateStats(Stats newStats)
	{
		stats = newStats;
		if (stats.skill < 0.0)
		{
			stats.skill = 0.0;
		}
	}

	private void UpdatePurchaseables(List<Purchaseable> purchaseableList)
	{
		purchaseables.Clear();
		foreach (Purchaseable purchaseable in purchaseableList)
		{
			purchaseables.Add(purchaseable.item_id, purchaseable);
		}
	}

	private void UpdateInventory(List<Item> itemList)
	{
		items_by_name.Clear();
		items_by_id.Clear();
		foreach (Item item in itemList)
		{
			items_by_name.Add(item.name, item);
			items_by_id.Add(item.id, item);
		}
		calculateItemPropertyMaximums(itemList);
	}

	private void calculateItemPropertyMaximums(List<Item> itemList)
	{
		foreach (Item item in itemList)
		{
			for (int i = 0; i < item.stats.Length; i++)
			{
				if (item.properties.ContainsKey(item.stats[i].value))
				{
					if (!propertyMaximums.ContainsKey(item.stats[i].name))
					{
						propertyMaximums.Add(item.stats[i].name, item.properties[item.stats[i].value]);
					}
					else if (propertyMaximums[item.stats[i].name] < item.properties[item.stats[i].value])
					{
						propertyMaximums[item.stats[i].name] = item.properties[item.stats[i].value];
					}
				}
			}
		}
	}

	private void UpdateServers(List<Server> serverList)
	{
		string empty = string.Empty;
		empty = "android_eight_person";
		servers.Clear();
		foreach (Server server in serverList)
		{
			if (!servers.ContainsKey(server.server_type))
			{
				servers.Add(server.server_type, new List<Server>());
			}
			if (!server.url.Contains("http://"))
			{
				server.url = "http://" + server.url;
			}
			if (server.server_type == "match")
			{
				if (server.description.Contains(empty))
				{
					servers[server.server_type].Add(server);
				}
				else if (server.description == "royale")
				{
					if (!servers.ContainsKey("royale"))
					{
						servers.Add("royale", new List<Server>());
					}
					servers["royale"].Add(server);
				}
			}
			else
			{
				servers[server.server_type].Add(server);
			}
		}
	}

	private void UpdateTips(List<string> tipList)
	{
		tips = tipList;
	}

	private void UpdateRewards(Dictionary<string, Reward> r)
	{
		rewards = r;
	}

	private void UpdatePlayerRewards(Dictionary<string, int> p)
	{
		player_rewards = p;
	}

	private void UpdateDeals(List<Deal> d)
	{
		deals = d;
	}

	private void UpdateCrossPromotionItems(List<CrossPromotionItem> c)
	{
		crossPromotionItems = c;
	}

	private void UpdateDailyReward(int d)
	{
		daily_reward = d;
	}

	private void UpdateProperties(Dictionary<string, string> props)
	{
		if (props.ContainsKey("apple_test_url_2_0"))
		{
			ROOT_SERVER_URL = props["apple_test_url_2_0"];
			RefreshServers(null, null);
		}
		properties = props;
	}

	private IEnumerator UpdateITunesProducts(Dictionary<string, int> productIds, FinishedCallback success, FinishedCallback failure)
	{
		iTunesProductIDs = productIds;
		string[] keys = new string[productIds.Keys.Count];
		productIds.Keys.CopyTo(keys, 0);
		if (success != null)
		{
			success();
		}
		yield return null;
	}

	private IEnumerator UpdateGooglePlayProducts(Dictionary<string, int> productIds, FinishedCallback success, FinishedCallback failure)
	{
		googlePlayProductIDs = productIds;
		string[] keys = new string[productIds.Keys.Count];
		productIds.Keys.CopyTo(keys, 0);
		GoogleIAB.queryInventory(keys);
		if (success != null)
		{
			success();
		}
		yield return null;
	}

	private IEnumerator UpdateMicrosoftStoreProducts(Dictionary<string, int> productIds, FinishedCallback success, FinishedCallback failure)
	{
		microsoftStoreProductIds = productIds;
		if (success != null)
		{
			success();
		}
		yield return null;
	}

	private void UpdateJouleExchangeRates(Dictionary<string, int> exchangeRates)
	{
		jouleExchangeRates.Clear();
		foreach (KeyValuePair<string, int> exchangeRate in exchangeRates)
		{
			int result = 0;
			if (int.TryParse(exchangeRate.Key, out result))
			{
				jouleExchangeRates.Add(result, exchangeRate.Value);
				continue;
			}
			Debug.LogWarning("Key was not an int, " + exchangeRate.Key + " = " + exchangeRate.Value);
		}
	}

	public bool UpdateProperty(string name, ref string val)
	{
		if (properties.ContainsKey(name))
		{
			val = properties[name];
			return true;
		}
		return false;
	}

	public bool UpdateProperty(string name, ref float val)
	{
		if (properties.ContainsKey(name))
		{
			return float.TryParse(properties[name], out val);
		}
		return false;
	}

	public bool UpdateProperty(string name, ref int val)
	{
		if (properties.ContainsKey(name))
		{
			return int.TryParse(properties[name], out val);
		}
		return false;
	}

	public bool UpdateProperty(string name, ref bool val)
	{
		if (properties.ContainsKey(name))
		{
			return bool.TryParse(properties[name], out val);
		}
		return false;
	}

	public void OverwriteProperty(string name, string val)
	{
		if (properties.ContainsKey(name))
		{
			properties[name] = val;
		}
	}

	public PictureLink[] GetRotatingPictureLinks()
	{
		string empty = string.Empty;
		empty = "android";
		int num = 0;
		foreach (CrossPromotionItem crossPromotionItem in crossPromotionItems)
		{
			if (crossPromotionItem.platform == empty)
			{
				num++;
			}
		}
		PictureLink[] array = new PictureLink[num];
		int num2 = 0;
		foreach (CrossPromotionItem crossPromotionItem2 in crossPromotionItems)
		{
			if (crossPromotionItem2.platform == empty)
			{
				array[num2] = new PictureLink(crossPromotionItem2.destination_url, crossPromotionItem2.name, crossPromotionItem2.button_image_url);
				num2++;
			}
		}
		return array;
	}

	public int GetGasCansPerPurchase(string productID)
	{
		int value;
		if (googlePlayProductIDs.TryGetValue(productID, out value))
		{
			if (productID.Contains("gascan"))
			{
				return value;
			}
			return -1;
		}
		return -1;
	}

	public Dictionary<int, int> GetJouleExchangeRates()
	{
		return new Dictionary<int, int>(jouleExchangeRates);
	}

	public int GetJoulesExchangeRate(int gasCanSetSize)
	{
		if (jouleExchangeRates.ContainsKey(gasCanSetSize))
		{
			return jouleExchangeRates[gasCanSetSize];
		}
		return -1;
	}

	public Dictionary<string, int> GetItunesProducts()
	{
		return new Dictionary<string, int>(iTunesProductIDs);
	}

	public Dictionary<string, int> GetGooglePlayProducts()
	{
		return new Dictionary<string, int>(googlePlayProductIDs);
	}

	public Dictionary<string, int> GetMicrosoftStoreProducts()
	{
		return new Dictionary<string, int>(microsoftStoreProductIds);
	}
}
