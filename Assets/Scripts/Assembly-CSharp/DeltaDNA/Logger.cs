using UnityEngine;

namespace DeltaDNA
{
	public static class Logger
	{
		public enum Level
		{
			DEBUG = 0,
			INFO = 1,
			WARNING = 2,
			ERROR = 3
		}

		public const string PREFIX = "[DDSDK] ";

		private static Level sLogLevel = Level.INFO;

		internal static Level LogLevel
		{
			get
			{
				return sLogLevel;
			}
		}

		public static void SetLogLevel(Level logLevel)
		{
			sLogLevel = logLevel;
		}

		internal static void LogDebug(string msg)
		{
			if (sLogLevel <= Level.DEBUG)
			{
				Log(msg, Level.DEBUG);
			}
		}

		internal static void LogInfo(string msg)
		{
			if (sLogLevel <= Level.INFO)
			{
				Log(msg, Level.INFO);
			}
		}

		internal static void LogWarning(string msg)
		{
			if (sLogLevel <= Level.WARNING)
			{
				Log(msg, Level.WARNING);
			}
		}

		internal static void LogError(string msg)
		{
			if (sLogLevel <= Level.ERROR)
			{
				Log(msg, Level.ERROR);
			}
		}

		private static void Log(string msg, Level level)
		{
			switch (level)
			{
			case Level.ERROR:
				Debug.LogError("[DDSDK] [ERROR] " + msg);
				break;
			case Level.WARNING:
				Debug.LogWarning("[DDSDK] [WARNING] " + msg);
				break;
			case Level.INFO:
				Debug.Log("[DDSDK] [INFO] " + msg);
				break;
			default:
				Debug.Log("[DDSDK] [DEBUG] " + msg);
				break;
			}
		}

		internal static void HandleLog(string logString, string stackTrace, LogType type)
		{
		}
	}
}
