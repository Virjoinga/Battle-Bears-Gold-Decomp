using UnityEngine;

namespace Utils
{
	public static class PrefsHelper
	{
		public static void SetBool(string key, bool val)
		{
			SetInt(key, val ? 1 : 0);
		}

		public static void SetInt(string key, int val)
		{
			PlayerPrefs.SetInt(KeyWithPrefix(key), val);
		}

		public static void SetFloat(string key, float val)
		{
			PlayerPrefs.SetFloat(KeyWithPrefix(key), val);
		}

		public static float GetFloat(string key, float defaultValue)
		{
			return PlayerPrefs.GetFloat(KeyWithPrefix(key), defaultValue);
		}

		public static int GetInt(string key, int defaultValue)
		{
			return PlayerPrefs.GetInt(KeyWithPrefix(key), defaultValue);
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			return GetInt(key, defaultValue ? 1 : 0) == 1;
		}

		private static string KeyWithPrefix(string key)
		{
			Stats stats = ServiceManager.Instance.GetStats();
			if (stats != null && stats.pid != -1)
			{
				return key + stats.pid;
			}
			return key;
		}
	}
}
