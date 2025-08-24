using System;
using System.Collections.Generic;
using DeltaDNA.MiniJSON;

namespace DeltaDNA
{
	internal class EngageRequest
	{
		public string DecisionPoint { get; private set; }

		public string Flavour { get; set; }

		public Dictionary<string, object> Parameters { get; set; }

		public EngageRequest(string decisionPoint)
		{
			DecisionPoint = decisionPoint;
			Flavour = "engagement";
			Parameters = new Dictionary<string, object>();
		}

		public string ToJSON()
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("userID", Singleton<DDNA>.Instance.UserID);
				dictionary.Add("decisionPoint", DecisionPoint);
				dictionary.Add("flavour", Flavour);
				dictionary.Add("sessionID", Singleton<DDNA>.Instance.SessionID);
				dictionary.Add("version", Settings.ENGAGE_API_VERSION);
				dictionary.Add("sdkVersion", Settings.SDK_VERSION);
				dictionary.Add("platform", Singleton<DDNA>.Instance.Platform);
				dictionary.Add("timezoneOffset", Convert.ToInt32(ClientInfo.TimezoneOffset));
				Dictionary<string, object> dictionary2 = dictionary;
				if (ClientInfo.Locale != null)
				{
					dictionary2.Add("locale", ClientInfo.Locale);
				}
				if (Parameters != null && Parameters.Count > 0)
				{
					dictionary2.Add("parameters", Parameters);
				}
				return Json.Serialize(dictionary2);
			}
			catch (Exception ex)
			{
				Logger.LogError("Error serialising engage request: " + ex.Message);
				return null;
			}
		}

		public override string ToString()
		{
			return string.Format("[EngageRequest]" + DecisionPoint + "(" + Flavour + ")\n" + Parameters);
		}
	}
}
