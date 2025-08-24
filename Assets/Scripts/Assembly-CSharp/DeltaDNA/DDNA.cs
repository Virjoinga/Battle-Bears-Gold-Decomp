using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DeltaDNA.MiniJSON;
using UnityEngine;

namespace DeltaDNA
{
	public class DDNA : Singleton<DDNA>
	{
		private static readonly string PF_KEY_USER_ID = "DDSDK_USER_ID";

		private bool started;

		private string collectURL;

		private string engageURL;

		private EventStore eventStore;

		private GameEvent launchNotificationEvent;

		private string pushNotificationToken;

		private string androidRegistrationId;

		private DateTime lastActive = DateTime.MinValue;

		private static Func<DateTime?> TimestampFunc = DefaultTimestampFunc;

		private static object _lock = new object();

		public Settings Settings { get; set; }

		public IosNotifications IosNotifications { get; private set; }

		public AndroidNotifications AndroidNotifications { get; private set; }

		public string EnvironmentKey { get; private set; }

		public string CollectURL
		{
			get
			{
				return collectURL;
			}
			private set
			{
				collectURL = ValidateURL(value);
			}
		}

		public string EngageURL
		{
			get
			{
				return engageURL;
			}
			private set
			{
				engageURL = ValidateURL(value);
			}
		}

		public string SessionID { get; private set; }

		public string UserID
		{
			get
			{
				string @string = PlayerPrefs.GetString(PF_KEY_USER_ID, null);
				if (string.IsNullOrEmpty(@string))
				{
					return null;
				}
				return @string;
			}
			private set
			{
				if (!string.IsNullOrEmpty(value))
				{
					PlayerPrefs.SetString(PF_KEY_USER_ID, value);
					PlayerPrefs.Save();
				}
			}
		}

		public bool HasStarted
		{
			get
			{
				return started;
			}
		}

		public bool IsUploading { get; private set; }

		public string HashSecret { get; set; }

		public string ClientVersion { get; set; }

		public string Platform { get; set; }

		public string PushNotificationToken
		{
			get
			{
				return pushNotificationToken;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value != pushNotificationToken)
				{
					GameEvent gameEvent = new GameEvent("notificationServices").AddParam("pushNotificationToken", value);
					if (started)
					{
						RecordEvent(gameEvent);
					}
					pushNotificationToken = value;
				}
			}
		}

		public string AndroidRegistrationID
		{
			get
			{
				return androidRegistrationId;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value != androidRegistrationId)
				{
					GameEvent gameEvent = new GameEvent("notificationServices").AddParam("androidRegistrationID", value);
					if (started)
					{
						RecordEvent(gameEvent);
					}
					androidRegistrationId = value;
				}
			}
		}

		public event Action OnNewSession;

		protected DDNA()
		{
			Settings = new Settings();
		}

		public void Init()
		{
		}

		private void Awake()
		{
			if (eventStore == null)
			{
				string text = null;
				if (Settings.UseEventStore)
				{
					text = Settings.EVENT_STORAGE_PATH.Replace("{persistent_path}", Application.persistentDataPath);
					if (!Utils.IsDirectoryWritable(text))
					{
						Logger.LogWarning("Event store path unwritable, event caching disabled.");
						Settings.UseEventStore = false;
					}
				}
				eventStore = new EventStore(text);
				if (Settings.UseEventStore && !eventStore.IsInitialised)
				{
					Logger.LogWarning("Failed to access event store path, event caching disabled.");
					Settings.UseEventStore = false;
					eventStore = new EventStore(text);
				}
			}
			GameObject gameObject = new GameObject();
			IosNotifications = gameObject.AddComponent<IosNotifications>();
			gameObject.transform.parent = base.gameObject.transform;
			GameObject gameObject2 = new GameObject();
			AndroidNotifications = gameObject2.AddComponent<AndroidNotifications>();
			gameObject2.transform.parent = base.gameObject.transform;
		}

		public void StartSDK(string envKey, string collectURL, string engageURL)
		{
			StartSDK(envKey, collectURL, engageURL, null);
		}

		public void StartSDK(string envKey, string collectURL, string engageURL, string userID)
		{
			lock (_lock)
			{
				bool flag = false;
				if (string.IsNullOrEmpty(UserID))
				{
					flag = true;
					if (string.IsNullOrEmpty(userID))
					{
						userID = GenerateUserID();
					}
				}
				else if (!string.IsNullOrEmpty(userID) && UserID != userID)
				{
					flag = true;
				}
				UserID = userID;
				if (flag)
				{
					Logger.LogInfo("Starting DDNA SDK with new user " + UserID);
				}
				else
				{
					Logger.LogInfo("Starting DDNA SDK with existing user " + UserID);
				}
				EnvironmentKey = envKey;
				CollectURL = collectURL;
				EngageURL = engageURL;
				if (Platform == null)
				{
					Platform = ClientInfo.Platform;
				}
				started = true;
				NewSession();
				if (launchNotificationEvent != null)
				{
					RecordEvent(launchNotificationEvent);
					launchNotificationEvent = null;
				}
				TriggerDefaultEvents(flag);
				if (Settings.BackgroundEventUpload && !IsInvoking("Upload"))
				{
					InvokeRepeating("Upload", Settings.BackgroundEventUploadStartDelaySeconds, Settings.BackgroundEventUploadRepeatRateSeconds);
				}
			}
		}

		public void NewSession()
		{
			string text = GenerateSessionID();
			Logger.LogInfo("Starting new session " + text);
			SessionID = text;
			if (this.OnNewSession != null)
			{
				this.OnNewSession();
			}
		}

		public void StopSDK()
		{
			lock (_lock)
			{
				if (started)
				{
					Logger.LogInfo("Stopping DDNA SDK");
					RecordEvent("gameEnded");
					CancelInvoke();
					Upload();
					started = false;
				}
				else
				{
					Logger.LogDebug("SDK not running");
				}
			}
		}

		public void RecordEvent<T>(T gameEvent) where T : GameEvent<T>
		{
			if (!started)
			{
				throw new Exception("You must first start the SDK via the StartSDK method");
			}
			gameEvent.AddParam("platform", Platform);
			gameEvent.AddParam("sdkVersion", Settings.SDK_VERSION);
			Dictionary<string, object> dictionary = gameEvent.AsDictionary();
			dictionary["userID"] = UserID;
			dictionary["sessionID"] = SessionID;
			dictionary["eventUUID"] = Guid.NewGuid().ToString();
			string currentTimestamp = GetCurrentTimestamp();
			if (currentTimestamp != null)
			{
				dictionary["eventTimestamp"] = GetCurrentTimestamp();
			}
			try
			{
				string obj = Json.Serialize(dictionary);
				if (!eventStore.Push(obj))
				{
					Logger.LogWarning("Event store full, dropping '" + gameEvent.Name + "' event.");
				}
			}
			catch (Exception ex)
			{
				Logger.LogWarning("Unable to generate JSON for '" + gameEvent.Name + "' event. " + ex.Message);
			}
		}

		public void RecordEvent(string eventName)
		{
			GameEvent gameEvent = new GameEvent(eventName);
			RecordEvent(gameEvent);
		}

		public void RecordEvent(string eventName, Dictionary<string, object> eventParams)
		{
			GameEvent gameEvent = new GameEvent(eventName);
			foreach (string key in eventParams.Keys)
			{
				gameEvent.AddParam(key, eventParams[key]);
			}
			RecordEvent(gameEvent);
		}

		public void RequestEngagement(Engagement engagement, Action<Dictionary<string, object>> callback)
		{
			if (!started)
			{
				throw new Exception("You must first start the SDK via the StartSDK method.");
			}
			if (string.IsNullOrEmpty(EngageURL))
			{
				throw new Exception("Engage URL not configured.");
			}
			try
			{
				Dictionary<string, object> dictionary = engagement.AsDictionary();
				EngageRequest engageRequest = new EngageRequest(dictionary["decisionPoint"] as string);
				engageRequest.Flavour = dictionary["flavour"] as string;
				engageRequest.Parameters = dictionary["parameters"] as Dictionary<string, object>;
				EngageResponse response2 = delegate(string response, int statusCode, string error)
				{
					Dictionary<string, object> obj = new Dictionary<string, object>();
					if (response != null)
					{
						try
						{
							obj = Json.Deserialize(response) as Dictionary<string, object>;
						}
						catch (Exception ex2)
						{
							Logger.LogError("Engagement " + engagement.DecisionPoint + " responded with invalid JSON: " + ex2.Message);
						}
					}
					callback(obj);
				};
				StartCoroutine(Engage.Request(this, engageRequest, response2));
			}
			catch (Exception ex)
			{
				Logger.LogWarning("Engagement request failed: " + ex.Message);
			}
		}

		public void RequestEngagement(Engagement engagement, Action<Engagement> onCompleted, Action<Exception> onError)
		{
			if (!started)
			{
				throw new Exception("You must first start the SDK via the StartSDK method.");
			}
			if (string.IsNullOrEmpty(EngageURL))
			{
				throw new Exception("Engage URL not configured.");
			}
			try
			{
				Dictionary<string, object> dictionary = engagement.AsDictionary();
				EngageRequest engageRequest = new EngageRequest(dictionary["decisionPoint"] as string);
				engageRequest.Flavour = dictionary["flavour"] as string;
				engageRequest.Parameters = dictionary["parameters"] as Dictionary<string, object>;
				EngageResponse response2 = delegate(string response, int statusCode, string error)
				{
					engagement.Raw = response;
					engagement.StatusCode = statusCode;
					engagement.Error = error;
					onCompleted(engagement);
				};
				StartCoroutine(Engage.Request(this, engageRequest, response2));
			}
			catch (Exception ex)
			{
				Logger.LogWarning("Engagement request failed: " + ex.Message);
			}
		}

		public void RecordPushNotification(Dictionary<string, object> payload)
		{
			Logger.LogDebug("Received push notification: " + payload);
			GameEvent gameEvent = new GameEvent("notificationOpened");
			try
			{
				if (payload.ContainsKey("_ddId"))
				{
					gameEvent.AddParam("notificationId", Convert.ToInt64(payload["_ddId"]));
				}
				if (payload.ContainsKey("_ddName"))
				{
					gameEvent.AddParam("notificationName", payload["_ddName"]);
				}
				bool flag = false;
				if (payload.ContainsKey("_ddCampaign"))
				{
					gameEvent.AddParam("campaignId", Convert.ToInt64(payload["_ddCampaign"]));
					flag = true;
				}
				if (payload.ContainsKey("_ddCohort"))
				{
					gameEvent.AddParam("cohortId", Convert.ToInt64(payload["_ddCohort"]));
					flag = true;
				}
				if (flag && payload.ContainsKey("_ddCommunicationSender"))
				{
					gameEvent.AddParam("communicationSender", payload["_ddCommunicationSender"]);
					gameEvent.AddParam("communicationState", "OPEN");
				}
				if (payload.ContainsKey("_ddLaunch"))
				{
					gameEvent.AddParam("notificationLaunch", Convert.ToBoolean(payload["_ddLaunch"]));
				}
				if (payload.ContainsKey("_ddCampaign"))
				{
					gameEvent.AddParam("campaignId", Convert.ToInt64(payload["_ddCampaign"]));
				}
				if (payload.ContainsKey("_ddCohort"))
				{
					gameEvent.AddParam("cohortId", Convert.ToInt64(payload["_ddCohort"]));
				}
				gameEvent.AddParam("communicationState", "OPEN");
			}
			catch (Exception ex)
			{
				Logger.LogError("Error parsing push notification payload. " + ex.Message);
			}
			if (started)
			{
				RecordEvent(gameEvent);
			}
			else
			{
				launchNotificationEvent = gameEvent;
			}
		}

		public void Upload()
		{
			if (!started)
			{
				Logger.LogError("You must first start the SDK via the StartSDK method.");
			}
			else if (IsUploading)
			{
				Logger.LogWarning("Event upload already in progress, try again later.");
			}
			else
			{
				StartCoroutine(UploadCoroutine());
			}
		}

		public void SetLoggingLevel(Logger.Level level)
		{
			Logger.SetLogLevel(level);
		}

		public void ClearPersistentData()
		{
			if (HasStarted)
			{
				Logger.LogWarning("SDK has not been stopped before clearing persistent data");
			}
			PlayerPrefs.DeleteKey(PF_KEY_USER_ID);
			if (eventStore != null)
			{
				eventStore.ClearAll();
			}
			Engage.ClearCache();
		}

		public void UseCollectTimestamp(bool useCollect)
		{
			if (!useCollect)
			{
				SetTimestampFunc(DefaultTimestampFunc);
				return;
			}
			SetTimestampFunc(() => null);
		}

		public void SetTimestampFunc(Func<DateTime?> TimestampFunc)
		{
			DDNA.TimestampFunc = TimestampFunc;
		}

		public override void OnDestroy()
		{
			if (eventStore != null)
			{
				eventStore.FlushBuffers();
				eventStore.Dispose();
			}
			PlayerPrefs.Save();
			base.OnDestroy();
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				lastActive = DateTime.UtcNow;
				eventStore.FlushBuffers();
				PlayerPrefs.Save();
				return;
			}
			double totalSeconds = (DateTime.UtcNow - lastActive).TotalSeconds;
			if (totalSeconds > (double)Settings.SessionTimeoutSeconds)
			{
				lastActive = DateTime.MinValue;
				NewSession();
			}
		}

		private string GenerateSessionID()
		{
			return Guid.NewGuid().ToString();
		}

		private string GenerateUserID()
		{
			return Guid.NewGuid().ToString();
		}

		private static DateTime? DefaultTimestampFunc()
		{
			return DateTime.UtcNow;
		}

		private static string GetCurrentTimestamp()
		{
			DateTime? dateTime = TimestampFunc();
			if (dateTime.HasValue)
			{
				string text = dateTime.Value.ToString(Settings.EVENT_TIMESTAMP_FORMAT, CultureInfo.InvariantCulture);
				if (text.EndsWith(".1000"))
				{
					text = text.Replace(".1000", ".999");
				}
				return text;
			}
			return null;
		}

		private IEnumerator UploadCoroutine()
		{
			IsUploading = true;
			try
			{
				eventStore.Swap();
				List<string> events = eventStore.Read();
				if (events == null || events.Count <= 0)
				{
					yield break;
				}
				Logger.LogDebug("Starting event upload.");
				yield return StartCoroutine(PostEvents(resultCallback: delegate(bool succeeded, int statusCode)
				{
					if (succeeded)
					{
						Logger.LogDebug("Event upload successful.");
						eventStore.ClearOut();
					}
					else if (statusCode == 400)
					{
						Logger.LogDebug("Collect rejected events, possible corruption.");
						eventStore.ClearOut();
					}
					else
					{
						Logger.LogWarning("Event upload failed - try again later.");
					}
				}, events: events.ToArray()));
			}
			finally
			{
				IsUploading = false;
			}
		}

		private IEnumerator PostEvents(string[] events, Action<bool, int> resultCallback)
		{
			string bulkEvent = "{\"eventList\":[" + string.Join(",", events) + "]}";
			string url = ((HashSecret == null) ? FormatURI(Settings.COLLECT_URL_PATTERN, CollectURL, EnvironmentKey, null) : FormatURI(hash: GenerateHash(bulkEvent, HashSecret), uriPattern: Settings.COLLECT_HASH_URL_PATTERN, apiHost: CollectURL, envKey: EnvironmentKey));
			int attempts = 0;
			bool succeeded = false;
			int status = 0;
			Action<int, string, string> completionHandler = delegate(int statusCode, string data, string error)
			{
				if (statusCode > 0 && statusCode < 400)
				{
					succeeded = true;
				}
				else
				{
					Logger.LogDebug("Error posting events: " + error + " " + data);
				}
				status = statusCode;
			};
			HttpRequest request = new HttpRequest(url);
			request.HTTPMethod = HttpRequest.HTTPMethodType.POST;
			request.HTTPBody = bulkEvent;
			request.setHeader("Content-Type", "application/json");
			do
			{
				yield return StartCoroutine(Network.SendRequest(request, completionHandler));
				if (!succeeded)
				{
					int num;
					attempts = (num = attempts + 1);
					if (num < Settings.HttpRequestMaxRetries)
					{
						break;
					}
					yield return new WaitForSeconds(Settings.HttpRequestRetryDelaySeconds);
					continue;
				}
				break;
			}
			while (attempts < Settings.HttpRequestMaxRetries);
			resultCallback(succeeded, status);
		}

		internal string ResolveEngageURL(string httpBody)
		{
			if (httpBody != null && HashSecret != null)
			{
				string hash = GenerateHash(httpBody, HashSecret);
				return FormatURI(Settings.ENGAGE_HASH_URL_PATTERN, EngageURL, EnvironmentKey, hash);
			}
			return FormatURI(Settings.ENGAGE_URL_PATTERN, EngageURL, EnvironmentKey, null);
		}

		private static string FormatURI(string uriPattern, string apiHost, string envKey, string hash)
		{
			string text = uriPattern.Replace("{host}", apiHost);
			text = text.Replace("{env_key}", envKey);
			return text.Replace("{hash}", hash);
		}

		private static string ValidateURL(string url)
		{
			if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
			{
				url = "https://" + url;
			}
			return url;
		}

		private static string GenerateHash(string data, string secret)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data + secret);
			byte[] array = Utils.ComputeMD5Hash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		private void TriggerDefaultEvents(bool newPlayer)
		{
			if (Settings.OnFirstRunSendNewPlayerEvent && newPlayer)
			{
				Logger.LogDebug("Sending 'newPlayer' event");
				GameEvent gameEvent = new GameEvent("newPlayer");
				if (ClientInfo.CountryCode != null)
				{
					gameEvent.AddParam("userCountry", ClientInfo.CountryCode);
				}
				RecordEvent(gameEvent);
			}
			if (Settings.OnInitSendGameStartedEvent)
			{
				Logger.LogDebug("Sending 'gameStarted' event");
				GameEvent gameEvent2 = new GameEvent("gameStarted").AddParam("clientVersion", ClientVersion).AddParam("userLocale", ClientInfo.Locale);
				if (!string.IsNullOrEmpty(PushNotificationToken))
				{
					gameEvent2.AddParam("pushNotificationToken", PushNotificationToken);
				}
				if (!string.IsNullOrEmpty(AndroidRegistrationID))
				{
					gameEvent2.AddParam("androidRegistrationID", AndroidRegistrationID);
				}
				RecordEvent(gameEvent2);
			}
			if (Settings.OnInitSendClientDeviceEvent)
			{
				Logger.LogDebug("Sending 'clientDevice' event");
				GameEvent gameEvent3 = new GameEvent("clientDevice").AddParam("deviceName", ClientInfo.DeviceName).AddParam("deviceType", ClientInfo.DeviceType).AddParam("hardwareVersion", ClientInfo.DeviceModel)
					.AddParam("operatingSystem", ClientInfo.OperatingSystem)
					.AddParam("operatingSystemVersion", ClientInfo.OperatingSystemVersion)
					.AddParam("timezoneOffset", ClientInfo.TimezoneOffset)
					.AddParam("userLanguage", ClientInfo.LanguageCode);
				if (ClientInfo.Manufacturer != null)
				{
					gameEvent3.AddParam("manufacturer", ClientInfo.Manufacturer);
				}
				RecordEvent(gameEvent3);
			}
		}
	}
}
