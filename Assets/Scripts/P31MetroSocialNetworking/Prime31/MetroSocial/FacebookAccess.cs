using System;
using System.Collections.Generic;

namespace Prime31.MetroSocial
{
	public static class FacebookAccess
	{
		public static string accessToken { get; set; }

		public static string applicationId { get; set; }

		public static string lastErrorMessage { get; set; }

		public static void login(string[] permissions, Action<string> completionHandler)
		{
		}

		public static void logout()
		{
		}

		public static bool isSessionValid()
		{
			return false;
		}

		public static void graphRequestGet(string path, Dictionary<string, object> parameters, Action<object> completionHandler)
		{
		}

		public static void graphRequestPost(string path, Dictionary<string, object> parameters, Action<object> completionHandler)
		{
		}

		public static void graphRequestDelete(string path, Dictionary<string, object> parameters, Action<object> completionHandler)
		{
		}
	}
}
