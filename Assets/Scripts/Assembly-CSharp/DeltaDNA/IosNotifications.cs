using System;
using System.Collections.Generic;
using DeltaDNA.MiniJSON;
using UnityEngine;

namespace DeltaDNA
{
	public class IosNotifications : MonoBehaviour
	{
		public event Action<string> OnDidLaunchWithPushNotification;

		public event Action<string> OnDidReceivePushNotification;

		public event Action<string> OnDidRegisterForPushNotifications;

		public event Action<string> OnDidFailToRegisterForPushNotifications;

		private void Awake()
		{
			base.gameObject.name = GetType().ToString();
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		public void RegisterForPushNotifications()
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}

		public void UnregisterForPushNotifications()
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}

		public void DidReceivePushNotification(string notification)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(notification) as Dictionary<string, object>;
			dictionary["_ddCommunicationSender"] = "APPLE_NOTIFICATION";
			bool? flag = dictionary["_ddLaunch"] as bool?;
			if (flag.HasValue && flag.Value)
			{
				Logger.LogDebug("Did launch with iOS push notification");
				Singleton<DDNA>.Instance.RecordPushNotification(dictionary);
				if (this.OnDidLaunchWithPushNotification != null)
				{
					this.OnDidLaunchWithPushNotification(notification);
				}
			}
			else
			{
				Logger.LogDebug("Did receive iOS push notification");
				Singleton<DDNA>.Instance.RecordPushNotification(dictionary);
				if (this.OnDidReceivePushNotification != null)
				{
					this.OnDidReceivePushNotification(notification);
				}
			}
		}

		public void DidRegisterForPushNotifications(string deviceToken)
		{
			Logger.LogInfo("Did register for iOS push notifications: " + deviceToken);
			Singleton<DDNA>.Instance.PushNotificationToken = deviceToken;
			if (this.OnDidRegisterForPushNotifications != null)
			{
				this.OnDidRegisterForPushNotifications(deviceToken);
			}
		}

		public void DidFailToRegisterForPushNotifications(string error)
		{
			Logger.LogWarning("Did fail to register for iOS push notifications: " + error);
			if (this.OnDidFailToRegisterForPushNotifications != null)
			{
				this.OnDidFailToRegisterForPushNotifications(error);
			}
		}
	}
}
