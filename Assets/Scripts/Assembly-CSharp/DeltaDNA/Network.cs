using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DeltaDNA
{
	internal static class Network
	{
		private const string HeaderKey = "STATUS";

		private const string StatusRegex = "^.*\\s(\\d{3})\\s.*$";

		private const string ErrorRegex = "^(\\d{3})\\s.*$";

		internal static IEnumerator SendRequest(HttpRequest request, Action<int, string, string> completionHandler)
		{
			WWW www;
			if (request.HTTPMethod == HttpRequest.HTTPMethodType.POST)
			{
				Dictionary<string, string> headers = new Dictionary<string, string>();
				WWWForm form = new WWWForm();
				foreach (KeyValuePair<string, string> entry2 in Utils.HashtableToDictionary<string, string>(form.headers))
				{
					headers[entry2.Key] = entry2.Value;
				}
				foreach (KeyValuePair<string, string> entry in request.getHeaders())
				{
					headers[entry.Key] = entry.Value;
				}
				www = new WWW(postData: Encoding.UTF8.GetBytes(request.HTTPBody), url: request.URL, headers: headers);
			}
			else
			{
				www = new WWW(request.URL);
			}
			float timer = 0f;
			bool timedout = false;
			while (!www.isDone)
			{
				if (timer > (float)request.TimeoutSeconds)
				{
					timedout = true;
					break;
				}
				timer += Time.deltaTime;
				yield return null;
			}
			int statusCode = 1001;
			string data = null;
			string error2 = null;
			if (timedout)
			{
				www.Dispose();
				error2 = "connect() timed out";
			}
			else
			{
				statusCode = ReadStatusCode(www);
				data = www.text;
				error2 = www.error;
			}
			if (completionHandler != null)
			{
				completionHandler(statusCode, data, error2);
			}
		}

		private static int ReadStatusCode(WWW www)
		{
			int result = 200;
			if (www.responseHeaders.ContainsKey("STATUS"))
			{
				MatchCollection matchCollection = Regex.Matches(www.responseHeaders["STATUS"], "^.*\\s(\\d{3})\\s.*$");
				if (matchCollection.Count > 0 && matchCollection[0].Groups.Count > 0)
				{
					result = Convert.ToInt32(matchCollection[0].Groups[1].Value);
				}
			}
			else if (!string.IsNullOrEmpty(www.error))
			{
				MatchCollection matchCollection2 = Regex.Matches(www.error, "^(\\d{3})\\s.*$");
				if (matchCollection2.Count > 0 && matchCollection2[0].Groups.Count > 0)
				{
					result = Convert.ToInt32(matchCollection2[0].Groups[1].Value);
				}
				else if (Application.platform == RuntimePlatform.WebGLPlayer)
				{
					Logger.LogDebug("IE11 Webplayer workaround, assuming request succeeded");
					result = 204;
				}
				else
				{
					result = 1002;
				}
			}
			else if (string.IsNullOrEmpty(www.text))
			{
				result = 204;
			}
			return result;
		}
	}
}
