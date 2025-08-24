using System;
using System.Collections;
using UnityEngine;

namespace DeltaDNA
{
	internal class Engage
	{
		internal static IEnumerator Request(MonoBehaviour caller, EngageRequest request, EngageResponse response)
		{
			string requestJSON = request.ToJSON();
			string url = Singleton<DDNA>.Instance.ResolveEngageURL(requestJSON);
			HttpRequest httpRequest = new HttpRequest(url);
			httpRequest.HTTPMethod = HttpRequest.HTTPMethodType.POST;
			httpRequest.HTTPBody = requestJSON;
			httpRequest.TimeoutSeconds = Singleton<DDNA>.Instance.Settings.HttpRequestEngageTimeoutSeconds;
			httpRequest.setHeader("Content-Type", "application/json");
			EngageRequest request2 = default(EngageRequest);
			EngageResponse response2 = default(EngageResponse);
			Action<int, string, string> httpHandler = delegate(int statusCode, string data, string error)
			{
				string key = "DDSDK_ENGAGEMENT_" + request2.DecisionPoint + "_" + request2.Flavour;
				if (error == null && statusCode >= 200 && statusCode < 300)
				{
					try
					{
						PlayerPrefs.SetString(key, data);
					}
					catch (Exception ex)
					{
						Logger.LogWarning("Unable to cache engagement: " + ex.Message);
					}
				}
				else
				{
					Logger.LogDebug("Engagement failed with " + statusCode + " " + error);
					if (PlayerPrefs.HasKey(key))
					{
						Logger.LogDebug("Using cached response");
						data = "{\"isCachedResponse\":true," + PlayerPrefs.GetString(key).Substring(1);
					}
					else
					{
						data = "{}";
					}
				}
				response2(data, statusCode, error);
			};
			yield return caller.StartCoroutine(Network.SendRequest(httpRequest, httpHandler));
		}

		internal static void ClearCache()
		{
		}
	}
}
