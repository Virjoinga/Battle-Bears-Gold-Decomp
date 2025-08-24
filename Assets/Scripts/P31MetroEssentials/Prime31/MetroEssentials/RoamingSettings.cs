using System;

namespace Prime31.MetroEssentials
{
	public static class RoamingSettings
	{
		public static string roamingFolder { get; set; }

		public static ulong roamingStorageQuota { get; set; }

		public static event Action dataChangedEvent;

		public static void clearAllApplicationData()
		{
		}

		public static void setValueForKey(string key, object value)
		{
		}

		public static object valueForKey(string key)
		{
			return null;
		}

		public static object valueForKey(string key, object defaultValue)
		{
			return null;
		}

		public static void deleteValueForKey(string key)
		{
		}

		public static string[] allKeys()
		{
			return null;
		}

		public static void setValueForKeyInContainer(string containerName, string key, object value)
		{
		}

		public static object valueForKeyInContainer(string containerName, string key)
		{
			return null;
		}

		public static object valueForKeyInContainer(string containerName, string key, object defaultValue)
		{
			return null;
		}

		public static void deleteValueForKeyInContainer(string containerName, string key)
		{
		}

		public static void deleteContainer(string containerName)
		{
		}

		public static string[] allContainers()
		{
			return null;
		}
	}
}
