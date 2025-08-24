using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TapjoyUnity.Internal
{
	public sealed class ApiBindingIos : ApiBinding
	{
		private ApiBindingIos()
			: base("iOS")
		{
			Tapjoy_SetUnityVersion("11.9.1");
		}

		public static void Install()
		{
			ApiBinding.SetInstance(new ApiBindingIos());
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
						transferDictionaryToObjectiveCWithName(dictionary, key);
						Tapjoy_SetKeyToDictionaryRefValueInDictionary(flag.Key, key, "connectFlags");
					}
					else
					{
						Tapjoy_SetKeyToValueInDictionary(flag.Key, flag.Value.ToString(), "connectFlags");
					}
				}
			}
			Tapjoy_Connect(sdkKey);
		}

		private void transferDictionaryToObjectiveCWithName(Dictionary<string, object> dictionary, string dictionaryName)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				Tapjoy_SetKeyToValueInDictionary(valueToSet: (item.Value.GetType() != typeof(bool)) ? item.Value.ToString() : ((!Convert.ToBoolean(item.Value.ToString())) ? "false" : "true"), key: item.Key, dictionaryName: dictionaryName);
			}
		}

		public override void ActionComplete(string actionID)
		{
			Tapjoy_ActionComplete(actionID);
		}

		public override string GetSDKVersion()
		{
			return Tapjoy_GetSDKVersion();
		}

		public override void SetDebugEnabled(bool enabled)
		{
			Tapjoy_SetDebugEnabled(enabled);
		}

		public override void SetGcmSender(string senderId)
		{
		}

		public override void SetAppDataVersion(string dataVersion)
		{
			Tapjoy_SetAppDataVersion(dataVersion);
		}

		public override void ActivateUnitySupport()
		{
		}

		public override void GetCurrencyBalance()
		{
			Tapjoy_GetCurrencyBalance();
		}

		public override void SpendCurrency(int amount)
		{
			Tapjoy_SpendCurrency(amount);
		}

		public override void AwardCurrency(int amount)
		{
			Tapjoy_AwardCurrency(amount);
		}

		public override float GetCurrencyMultiplier()
		{
			return Tapjoy_GetCurrencyMultiplier();
		}

		public override void SetCurrencyMultiplier(float multiplier)
		{
			Tapjoy_SetCurrencyMultiplier(multiplier);
		}

		public override void ShowDefaultEarnedCurrencyAlert()
		{
			Tapjoy_ShowDefaultEarnedCurrencyAlert();
		}

		public override void CreatePlacement(string placementGuid, string eventName)
		{
			Tapjoy_CreatePlacement(placementGuid, eventName);
		}

		public override void DismissPlacementContent()
		{
			Tapjoy_DismissPlacementContent();
		}

		public override void RequestPlacementContent(string placementGuid)
		{
			Tapjoy_RequestPlacementContent(placementGuid);
		}

		public override void ShowPlacementContent(string placementGuid)
		{
			Tapjoy_ShowPlacementContent(placementGuid);
		}

		public override bool IsPlacementContentReady(string placementGuid)
		{
			return Tapjoy_IsPlacementContentReady(placementGuid);
		}

		public override bool IsPlacementContentAvailable(string placementGuid)
		{
			return Tapjoy_IsPlacementContentAvailable(placementGuid);
		}

		public override void ActionRequestCompleted(string requestId)
		{
			Tapjoy_ActionRequestCompleted(requestId);
		}

		public override void ActionRequestCancelled(string requestId)
		{
			Tapjoy_ActionRequestCancelled(requestId);
		}

		public override void RemovePlacement(string placementGuid)
		{
			Tapjoy_RemovePlacement(placementGuid);
		}

		public override void RemoveActionRequest(string requestID)
		{
			Tapjoy_RemoveActionRequest(requestID);
		}

		public override void EnablePaidAppWithActionID(string enablePaidAppWithActionID)
		{
		}

		public override void StartSession()
		{
			Tapjoy_StartSession();
		}

		public override void EndSession()
		{
			Tapjoy_EndSession();
		}

		public override void SetUserID(string userID)
		{
			Tapjoy_SetUserID(userID);
		}

		public override void SetUserLevel(int userLevel)
		{
			Tapjoy_SetUserLevel(userLevel);
		}

		public override void SetUserFriendCount(int friendCount)
		{
			Tapjoy_SetUserFriendCount(friendCount);
		}

		public override void SetUserCohortVariable(int variableIndex, string value)
		{
			Tapjoy_SetUserCohortVariable(variableIndex, value);
		}

		public override void ClearUserTags()
		{
			Tapjoy_ClearUserTags();
		}

		public override void AddUserTag(string tag)
		{
			Tapjoy_AddUserTag(tag);
		}

		public override void RemoveUserTag(string tag)
		{
			Tapjoy_RemoveUserTag(tag);
		}

		public override bool IsPushNotificationDisabled()
		{
			return false;
		}

		public override void SetPushNotificationDisabled(bool disabled)
		{
		}

		public override void TrackEvent(string name, long value)
		{
			Tapjoy_TrackEvent(null, name, null, null, "value", value, null, 0L, null, 0L);
		}

		public override void TrackEvent(string category, string name, long value)
		{
			Tapjoy_TrackEvent(category, name, null, null, "value", value, null, 0L, null, 0L);
		}

		public override void TrackEvent(string category, string name, string parameter1, string parameter2, long value)
		{
			Tapjoy_TrackEvent(category, name, parameter1, parameter2, "value", value, null, 0L, null, 0L);
		}

		public override void TrackEvent(string category, string name, string parameter1, string parameter2, string value1Name, long value1, string value2Name, long value2, string value3Name, long value3)
		{
			Tapjoy_TrackEvent(category, name, parameter1, parameter2, value1Name, value1, value2Name, value2, value3Name, value3);
		}

		public override void TrackPurchase(string productId, string currencyCode, double price, string campaignId)
		{
			Tapjoy_TrackPurchase(productId, currencyCode, price, campaignId, null);
		}

		public override void TrackPurchaseInGooglePlayStore(string skuDetails, string purchaseData, string dataSignature, string campaignId)
		{
		}

		public override void TrackPurchaseInAppleAppStore(string productId, string currencyCode, double productPrice, string transactionId, string campaignId)
		{
			Tapjoy_TrackPurchase(productId, currencyCode, productPrice, campaignId, transactionId);
		}

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetUnityVersion(string version);

		[DllImport("__Internal")]
		private static extern void Tapjoy_Connect(string sdkKey);

		[DllImport("__Internal")]
		private static extern void Tapjoy_ActionComplete(string actionID);

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetKeyToValueInDictionary(string key, string valueToSet, string dictionaryName);

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetKeyToDictionaryRefValueInDictionary(string key, string dictionaryNameToSet, string dictionaryNameToSetTo);

		[DllImport("__Internal")]
		private static extern string Tapjoy_GetSDKVersion();

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetDebugEnabled(bool enabled);

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetAppDataVersion(string dataVersion);

		[DllImport("__Internal")]
		private static extern void Tapjoy_GetCurrencyBalance();

		[DllImport("__Internal")]
		private static extern void Tapjoy_SpendCurrency(int amount);

		[DllImport("__Internal")]
		private static extern void Tapjoy_AwardCurrency(int amount);

		[DllImport("__Internal")]
		private static extern float Tapjoy_GetCurrencyMultiplier();

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetCurrencyMultiplier(float multiplier);

		[DllImport("__Internal")]
		private static extern void Tapjoy_ShowDefaultEarnedCurrencyAlert();

		[DllImport("__Internal")]
		private static extern void Tapjoy_CreatePlacement(string placementGuid, string eventName);

		[DllImport("__Internal")]
		private static extern void Tapjoy_DismissPlacementContent();

		[DllImport("__Internal")]
		private static extern void Tapjoy_RequestPlacementContent(string placementGuid);

		[DllImport("__Internal")]
		private static extern void Tapjoy_ShowPlacementContent(string placementGuid);

		[DllImport("__Internal")]
		private static extern bool Tapjoy_IsPlacementContentAvailable(string placementGuid);

		[DllImport("__Internal")]
		private static extern bool Tapjoy_IsPlacementContentReady(string placementGuid);

		[DllImport("__Internal")]
		private static extern void Tapjoy_ActionRequestCompleted(string requestId);

		[DllImport("__Internal")]
		private static extern void Tapjoy_ActionRequestCancelled(string requestId);

		[DllImport("__Internal")]
		private static extern void Tapjoy_RemovePlacement(string placementGuid);

		[DllImport("__Internal")]
		private static extern void Tapjoy_RemoveActionRequest(string requestId);

		[DllImport("__Internal")]
		private static extern void Tapjoy_StartSession();

		[DllImport("__Internal")]
		private static extern void Tapjoy_EndSession();

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetUserID(string userId);

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetUserLevel(int userLevel);

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetUserFriendCount(int friendCount);

		[DllImport("__Internal")]
		private static extern void Tapjoy_SetUserCohortVariable(int variableIndex, string value);

		[DllImport("__Internal")]
		private static extern void Tapjoy_ClearUserTags();

		[DllImport("__Internal")]
		private static extern void Tapjoy_AddUserTag(string tag);

		[DllImport("__Internal")]
		private static extern void Tapjoy_RemoveUserTag(string tag);

		[DllImport("__Internal")]
		private static extern void Tapjoy_TrackEvent(string category, string name, string parameter1, string parameter2, string value1Name, long value1, string value2Name, long value2, string value3Name, long value3);

		[DllImport("__Internal")]
		private static extern void Tapjoy_TrackPurchase(string productId, string currencyCode, double price, string campaignId, string transactionId);

		private static string GetStringFromNativeUtf8(IntPtr nativeUtf8)
		{
			int i;
			for (i = 0; Marshal.ReadByte(nativeUtf8, i) != 0; i++)
			{
			}
			if (i == 0)
			{
				return string.Empty;
			}
			byte[] array = new byte[i];
			Marshal.Copy(nativeUtf8, array, 0, array.Length);
			return Encoding.UTF8.GetString(array, 0, array.Length);
		}
	}
}
