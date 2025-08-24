using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TapjoyUnity.Internal
{
	public sealed class ApiBindingAndroid : ApiBinding
	{
		private AndroidJavaClass tapjoyJavaAPI;

		private AndroidJavaClass tjSupportClass;

		private bool canPassNullArgument;

		private ApiBindingAndroid()
			: base("Android")
		{
			tapjoyJavaAPI = new AndroidJavaClass("com.tapjoy.Tapjoy");
			tjSupportClass = new AndroidJavaClass("com.tapjoy.TapjoyConnectUnity");
			canPassNullArgument = CanPassNullArgument();
			tjSupportClass.CallStatic("activate", "11.9.1");
		}

		public static void Install()
		{
			ApiBinding.SetInstance(new ApiBindingAndroid());
		}

		private static bool CanPassNullArgument()
		{
			double result = 0.0;
			Match match = Regex.Match(Application.unityVersion, "^[0-9]+.[0-9]+");
			if (match.Success && double.TryParse(match.Groups[0].Value, out result) && result >= 4.1)
			{
				return true;
			}
			return false;
		}

		public override void Connect(string sdkKey, Dictionary<string, object> flags)
		{
			if (sdkKey == null)
			{
				return;
			}
			if (flags != null)
			{
				foreach (KeyValuePair<string, object> flag in flags)
				{
					if (flag.Value.GetType().IsGenericType)
					{
						Dictionary<string, object> dictionary = (Dictionary<string, object>)flag.Value;
						string key = flag.Key;
						transferDictionaryToJavaWithName(dictionary, key);
						tjSupportClass.CallStatic("setDictionaryInDictionary", flag.Key, key, "connectFlags");
					}
					else
					{
						tjSupportClass.CallStatic("setKeyValueInDictionary", flag.Key, flag.Value, "connectFlags");
					}
				}
			}
			tjSupportClass.CallStatic("connect", sdkKey);
		}

		private void transferDictionaryToJavaWithName(Dictionary<string, object> dictionary, string dictionaryName)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				tjSupportClass.CallStatic("setKeyValueInDictionary", item.Key, item.Value, dictionaryName);
			}
		}

		public override void ActionComplete(string actionID)
		{
			tapjoyJavaAPI.CallStatic("actionComplete", actionID);
		}

		public override string GetSDKVersion()
		{
			return tapjoyJavaAPI.CallStatic<string>("getVersion", new object[0]);
		}

		public override void SetDebugEnabled(bool enabled)
		{
			tapjoyJavaAPI.CallStatic("setDebugEnabled", enabled);
		}

		public override void SetGcmSender(string senderId)
		{
			senderId = MakeStringUnitySafe(senderId);
			tapjoyJavaAPI.CallStatic("setGcmSender", senderId);
		}

		public override void SetAppDataVersion(string dataVersion)
		{
			dataVersion = MakeStringUnitySafe(dataVersion);
			tapjoyJavaAPI.CallStatic("setAppDataVersion", dataVersion);
		}

		public override void ActivateUnitySupport()
		{
			tjSupportClass.CallStatic("activate", "11.9.1");
		}

		public override void AwardCurrency(int amount)
		{
			tjSupportClass.CallStatic("awardCurrency", amount);
		}

		public override void GetCurrencyBalance()
		{
			tjSupportClass.CallStatic("getCurrencyBalance");
		}

		public override void SpendCurrency(int amount)
		{
			tjSupportClass.CallStatic("spendCurrency", amount);
		}

		public override float GetCurrencyMultiplier()
		{
			return tjSupportClass.CallStatic<float>("getCurrencyMultiplier", new object[0]);
		}

		public override void SetCurrencyMultiplier(float multiplier)
		{
			tapjoyJavaAPI.CallStatic("setCurrencyMultiplier", multiplier);
		}

		public override void ShowDefaultEarnedCurrencyAlert()
		{
			tjSupportClass.CallStatic("showDefaultEarnedCurrencyAlert");
		}

		public override void CreatePlacement(string placementGuid, string placementName)
		{
			placementGuid = MakeStringUnitySafe(placementGuid);
			placementName = MakeStringUnitySafe(placementName);
			tjSupportClass.CallStatic("createPlacement", placementGuid, placementName);
		}

		public override void DismissPlacementContent()
		{
			tjSupportClass.CallStatic("dismissPlacementContent");
		}

		public override void RequestPlacementContent(string placementGuid)
		{
			placementGuid = MakeStringUnitySafe(placementGuid);
			tjSupportClass.CallStatic("requestPlacementContent", placementGuid);
		}

		public override void ShowPlacementContent(string placementGuid)
		{
			placementGuid = MakeStringUnitySafe(placementGuid);
			tjSupportClass.CallStatic("showPlacementContent", placementGuid);
		}

		public override bool IsPlacementContentReady(string placementGuid)
		{
			placementGuid = MakeStringUnitySafe(placementGuid);
			return tjSupportClass.CallStatic<bool>("isPlacementContentReady", new object[1] { placementGuid });
		}

		public override bool IsPlacementContentAvailable(string placementGuid)
		{
			placementGuid = MakeStringUnitySafe(placementGuid);
			return tjSupportClass.CallStatic<bool>("isPlacementContentAvailable", new object[1] { placementGuid });
		}

		public override void ActionRequestCompleted(string requestID)
		{
			requestID = MakeStringUnitySafe(requestID);
			tjSupportClass.CallStatic("actionRequestCompleted", requestID);
		}

		public override void ActionRequestCancelled(string requestID)
		{
			requestID = MakeStringUnitySafe(requestID);
			tjSupportClass.CallStatic("actionRequestCancelled", requestID);
		}

		public override void RemovePlacement(string placementGuid)
		{
			placementGuid = MakeStringUnitySafe(placementGuid);
			tjSupportClass.CallStatic("removePlacement", placementGuid);
		}

		public override void RemoveActionRequest(string requestID)
		{
			requestID = MakeStringUnitySafe(requestID);
			tjSupportClass.CallStatic("removeActionRequest", requestID);
		}

		public override void EnablePaidAppWithActionID(string enablePaidAppWithActionID)
		{
			tapjoyJavaAPI.CallStatic("enablePaidAppWithActionID", enablePaidAppWithActionID);
		}

		public override void StartSession()
		{
			tjSupportClass.CallStatic("onActivityStart");
		}

		public override void EndSession()
		{
			tjSupportClass.CallStatic("onActivityStop");
		}

		public override void SetUserID(string userId)
		{
			userId = MakeStringUnitySafe(userId);
			tjSupportClass.CallStatic("setUserID", userId);
		}

		public override void SetUserLevel(int userLevel)
		{
			tapjoyJavaAPI.CallStatic("setUserLevel", userLevel);
		}

		public override void SetUserFriendCount(int friendCount)
		{
			tapjoyJavaAPI.CallStatic("setUserFriendCount", friendCount);
		}

		public override void SetUserCohortVariable(int variableIndex, string value)
		{
			value = MakeStringUnitySafe(value);
			tapjoyJavaAPI.CallStatic("setUserCohortVariable", variableIndex, value);
		}

		public override void ClearUserTags()
		{
			tapjoyJavaAPI.CallStatic("clearUserTags");
		}

		public override void AddUserTag(string tag)
		{
			tag = MakeStringUnitySafe(tag);
			tapjoyJavaAPI.CallStatic("addUserTag", tag);
		}

		public override void RemoveUserTag(string tag)
		{
			tag = MakeStringUnitySafe(tag);
			tapjoyJavaAPI.CallStatic("removeUserTag", tag);
		}

		public override bool IsPushNotificationDisabled()
		{
			return tapjoyJavaAPI.CallStatic<bool>("isPushNotificationDisabled", new object[0]);
		}

		public override void SetPushNotificationDisabled(bool disabled)
		{
			tapjoyJavaAPI.CallStatic("setPushNotificationDisabled", disabled);
		}

		public override void TrackEvent(string name, long value)
		{
			TrackEvent(null, name, null, null, value);
		}

		public override void TrackEvent(string category, string name, long value)
		{
			TrackEvent(category, name, null, null, value);
		}

		public override void TrackEvent(string category, string name, string parameter1, string parameter2, long value)
		{
			category = MakeStringUnitySafe(category);
			name = MakeStringUnitySafe(name);
			parameter1 = MakeStringUnitySafe(parameter1);
			parameter2 = MakeStringUnitySafe(parameter2);
			tapjoyJavaAPI.CallStatic("trackEvent", category, name, parameter1, parameter2, value);
		}

		public override void TrackEvent(string category, string name, string parameter1, string parameter2, string value1Name, long value1, string value2Name, long value2, string value3Name, long value3)
		{
			category = MakeStringUnitySafe(category);
			name = MakeStringUnitySafe(name);
			parameter1 = MakeStringUnitySafe(parameter1);
			parameter2 = MakeStringUnitySafe(parameter2);
			value1Name = MakeStringUnitySafe(value1Name);
			value2Name = MakeStringUnitySafe(value2Name);
			value3Name = MakeStringUnitySafe(value3Name);
			tapjoyJavaAPI.CallStatic("trackEvent", category, name, parameter1, parameter2, value1Name, value1, value2Name, value2, value3Name, value3);
		}

		public override void TrackPurchase(string productId, string currencyCode, double price, string campaignId)
		{
			productId = MakeStringUnitySafe(productId);
			currencyCode = MakeStringUnitySafe(currencyCode);
			campaignId = MakeStringUnitySafe(campaignId);
			tapjoyJavaAPI.CallStatic("trackPurchase", productId, currencyCode, price, campaignId);
		}

		public override void TrackPurchaseInGooglePlayStore(string skuDetails, string purchaseData, string dataSignature, string campaignId)
		{
			skuDetails = MakeStringUnitySafe(skuDetails);
			purchaseData = MakeStringUnitySafe(purchaseData);
			dataSignature = MakeStringUnitySafe(dataSignature);
			campaignId = MakeStringUnitySafe(campaignId);
			tapjoyJavaAPI.CallStatic("trackPurchase", skuDetails, purchaseData, dataSignature, campaignId);
		}

		public override void TrackPurchaseInAppleAppStore(string productId, string currencyCode, double productPrice, string transactionId, string campaignId)
		{
		}

		private string MakeStringUnitySafe(string s)
		{
			if (s != null)
			{
				return s;
			}
			return (!canPassNullArgument) ? string.Empty : null;
		}
	}
}
