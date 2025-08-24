using System;
using System.Collections.Generic;

namespace Prime31.MetroSocial
{
	public static class TwitterAccess
	{
		public static string screenName { get; set; }

		public static string userId { get; set; }

		public static bool isLoggedIn { get; set; }

		public static string lastStatusMessage { get; set; }

		public static bool init(string consumerKey, string consumerSecret)
		{
			return false;
		}

		public static void login(Action<bool> completionHandler)
		{
		}

		public static void logout()
		{
		}

		public static void performGetRequest(string path, Dictionary<string, string> parameters, Action<object, string> completionHandler)
		{
		}

		public static void performPostRequest(string path, Dictionary<string, string> parameters, Action<object, string> completionHandler)
		{
		}
	}
}
