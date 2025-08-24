using System;
using System.Collections.Generic;
using DeltaDNA.Android;
using DeltaDNA.MiniJSON;
using UnityEngine;

namespace DeltaDNA
{
	public class AndroidNotifications : MonoBehaviour
	{
		private DDNANotifications ddnaNotifications;

		public event Action<string> OnDidLaunchWithPushNotification;

		public event Action<string> OnDidReceivePushNotification;

		public event Action<string> OnDidRegisterForPushNotifications;

		public event Action<string> OnDidFailToRegisterForPushNotifications;

		private void Awake()
		{
			base.gameObject.name = GetType().ToString();
			UnityEngine.Object.DontDestroyOnLoad(this);
			ddnaNotifications = new DDNANotifications();
			ddnaNotifications.MarkUnityLoaded();
		}

		public void RegisterForPushNotifications(bool secondary = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				try
				{
					ddnaNotifications.Register(new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"), secondary);
				}
				catch (AndroidJavaException ex)
				{
					Logger.LogWarning("Failed to register for push notifications. Notifications may not be configured correctly. " + ex.Message);
				}
			}
		}

		public void UnregisterForPushNotifications()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Singleton<DDNA>.Instance.AndroidRegistrationID = null;
			}
		}

		public void DidReceivePushNotification(string notification)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(notification) as Dictionary<string, object>;
			dictionary["_ddCommunicationSender"] = "GOOGLE_NOTIFICATION";
			bool? flag = dictionary["_ddLaunch"] as bool?;
			if (flag.HasValue && flag.Value)
			{
				Logger.LogDebug("Did launch with Android push notification");
				Singleton<DDNA>.Instance.RecordPushNotification(dictionary);
				if (this.OnDidLaunchWithPushNotification != null)
				{
					this.OnDidLaunchWithPushNotification(notification);
				}
			}
			else
			{
				Logger.LogDebug("Did receive Android push notification");
				Singleton<DDNA>.Instance.RecordPushNotification(dictionary);
				if (this.OnDidReceivePushNotification != null)
				{
					this.OnDidReceivePushNotification(notification);
				}
			}
		}

		public void DidRegisterForPushNotifications(string registrationId)
		{
			Logger.LogDebug("Did register for Android push notifications: " + registrationId);
			Singleton<DDNA>.Instance.AndroidRegistrationID = registrationId;
			if (this.OnDidRegisterForPushNotifications != null)
			{
				this.OnDidRegisterForPushNotifications(registrationId);
			}
		}

		public void DidFailToRegisterForPushNotifications(string error)
		{
			Logger.LogWarning("Did fail to register for Android push notifications: " + error);
			if (this.OnDidFailToRegisterForPushNotifications != null)
			{
				this.OnDidFailToRegisterForPushNotifications(error);
			}
		}
	}
}
