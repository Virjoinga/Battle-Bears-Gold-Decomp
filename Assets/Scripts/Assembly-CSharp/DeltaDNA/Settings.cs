namespace DeltaDNA
{
	public class Settings
	{
		internal static readonly string SDK_VERSION = "Unity SDK v4.6.0";

		internal static readonly string ENGAGE_API_VERSION = "4";

		internal static readonly string EVENT_STORAGE_PATH = "{persistent_path}/ddsdk/events/";

		internal static readonly string ENGAGE_STORAGE_PATH = "{persistent_path}/ddsdk/engage/";

		internal static readonly string LEGACY_SETTINGS_STORAGE_PATH = "{persistent_path}/GASettings.ini";

		internal static readonly string EVENT_TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

		internal static readonly string USERID_URL_PATTERN = "{host}/uuid";

		internal static readonly string COLLECT_URL_PATTERN = "{host}/{env_key}/bulk";

		internal static readonly string COLLECT_HASH_URL_PATTERN = "{host}/{env_key}/bulk/hash/{hash}";

		internal static readonly string ENGAGE_URL_PATTERN = "{host}/{env_key}";

		internal static readonly string ENGAGE_HASH_URL_PATTERN = "{host}/{env_key}/hash/{hash}";

		private bool _debugMode;

		public bool OnFirstRunSendNewPlayerEvent { get; set; }

		public bool OnInitSendClientDeviceEvent { get; set; }

		public bool OnInitSendGameStartedEvent { get; set; }

		public bool DebugMode
		{
			get
			{
				return _debugMode;
			}
			set
			{
				Logger.SetLogLevel((!value) ? Logger.Level.WARNING : Logger.Level.DEBUG);
				_debugMode = value;
			}
		}

		public float HttpRequestRetryDelaySeconds { get; set; }

		public int HttpRequestMaxRetries { get; set; }

		public int HttpRequestCollectTimeoutSeconds { get; set; }

		public int HttpRequestEngageTimeoutSeconds { get; set; }

		public bool BackgroundEventUpload { get; set; }

		public int BackgroundEventUploadStartDelaySeconds { get; set; }

		public int BackgroundEventUploadRepeatRateSeconds { get; set; }

		public bool UseEventStore { get; set; }

		public int SessionTimeoutSeconds { get; set; }

		internal Settings()
		{
			DebugMode = false;
			OnFirstRunSendNewPlayerEvent = true;
			OnInitSendClientDeviceEvent = true;
			OnInitSendGameStartedEvent = true;
			HttpRequestRetryDelaySeconds = 2f;
			HttpRequestMaxRetries = 0;
			HttpRequestCollectTimeoutSeconds = 30;
			HttpRequestEngageTimeoutSeconds = 5;
			BackgroundEventUpload = true;
			BackgroundEventUploadStartDelaySeconds = 0;
			BackgroundEventUploadRepeatRateSeconds = 60;
			UseEventStore = true;
			SessionTimeoutSeconds = 300;
		}
	}
}
