using System;
using System.Collections.Generic;
using DeltaDNA.MiniJSON;
using UnityEngine;
using UnityEngine.UI;

namespace DeltaDNA
{
	public class Example : MonoBehaviour
	{
		public const string ENVIRONMENT_KEY = "76410301326725846610230818914037";

		public const string COLLECT_URL = "http://collect2470ntysd.deltadna.net/collect/api";

		public const string ENGAGE_URL = "http://engage2470ntysd.deltadna.net";

		public const string ENGAGE_TEST_URL = "http://www.deltadna.net/qa/engage";

		[SerializeField]
		private Transform cubeObj;

		[SerializeField]
		private GameObject popUpObj;

		[SerializeField]
		private Text popUpContent;

		[SerializeField]
		private Text popUpTitle;

		private void Start()
		{
			Singleton<DDNA>.Instance.SetLoggingLevel(Logger.Level.DEBUG);
			Singleton<DDNA>.Instance.HashSecret = "1VLjWqChV2YC1sJ4EPKGzSF3TbhS26hq";
			Singleton<DDNA>.Instance.ClientVersion = "1.0.0";
			Singleton<DDNA>.Instance.IosNotifications.OnDidRegisterForPushNotifications += delegate(string n)
			{
				Debug.Log("Got an iOS push token: " + n);
			};
			Singleton<DDNA>.Instance.IosNotifications.OnDidReceivePushNotification += delegate(string n)
			{
				Debug.Log("Got an iOS push notification! " + n);
			};
			Singleton<DDNA>.Instance.IosNotifications.OnDidLaunchWithPushNotification += delegate(string n)
			{
				Debug.Log("Launched with an iOS push notification: " + n);
			};
			Singleton<DDNA>.Instance.IosNotifications.RegisterForPushNotifications();
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidRegisterForPushNotifications += delegate(string n)
			{
				Debug.Log("Got an Android registration token: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidFailToRegisterForPushNotifications += delegate(string n)
			{
				Debug.Log("Failed getting an Android registration token: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidReceivePushNotification += delegate(string n)
			{
				Debug.Log("Got an Android push notification: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.OnDidLaunchWithPushNotification += delegate(string n)
			{
				Debug.Log("Launched with an Android push notification: " + n);
			};
			Singleton<DDNA>.Instance.AndroidNotifications.RegisterForPushNotifications();
			Singleton<DDNA>.Instance.StartSDK("76410301326725846610230818914037", "http://collect2470ntysd.deltadna.net/collect/api", "http://engage2470ntysd.deltadna.net");
		}

		private void FixedUpdate()
		{
			if (Singleton<DDNA>.Instance.HasStarted)
			{
				cubeObj.Rotate(new Vector3(15f, 30f, 45f) * Time.deltaTime);
			}
		}

		public void OnSimpleEventBtn_Clicked()
		{
			GameEvent gameEvent = new GameEvent("options").AddParam("option", "sword").AddParam("action", "sell");
			Singleton<DDNA>.Instance.RecordEvent(gameEvent);
		}

		public void OnAchievementEventBtn_Clicked()
		{
			GameEvent gameEvent = new GameEvent("achievement").AddParam("achievementName", "Sunday Showdown Tournament Win").AddParam("achievementID", "SS-2014-03-02-01").AddParam("reward", new Params().AddParam("rewardName", "Medal").AddParam("rewardProducts", new Product().AddVirtualCurrency("VIP Points", "GRIND", 20L).AddItem("Sunday Showdown Medal", "Victory Badge", 1)));
			Singleton<DDNA>.Instance.RecordEvent(gameEvent);
		}

		public void OnTransactionEventBtn_Clicked()
		{
			Transaction gameEvent = new Transaction("Weapon type 11 manual repair", "PURCHASE", new Product().AddItem("WeaponsMaxConditionRepair:11", "WeaponMaxConditionRepair", 5).AddVirtualCurrency("Credit", "PREMIUM", 710L), new Product().SetRealCurrency("USD", Product<Product>.ConvertCurrency("USD", 12.34m))).SetTransactorId("2.212.91.84:15116").SetProductId("4019").AddParam("paymentCountry", "GB");
			Singleton<DDNA>.Instance.RecordEvent(gameEvent);
		}

		public void OnEngagementBtn_Clicked()
		{
			Engagement engagement = new Engagement("gameLoaded").AddParam("userLevel", 4).AddParam("experience", 1000).AddParam("missionName", "Disco Volante");
			Singleton<DDNA>.Instance.RequestEngagement(engagement, delegate(Dictionary<string, object> response)
			{
				popUpContent.text = Json.Serialize(response);
			});
			popUpTitle.text = "Engage returned";
			popUpObj.SetActive(true);
		}

		public void OnImageMessageBtn_Clicked()
		{
			Engagement engagement = new Engagement("testImageMessage").AddParam("userLevel", 4).AddParam("experience", 1000).AddParam("missionName", "Disco Volante");
			Singleton<DDNA>.Instance.RequestEngagement(engagement, delegate(Engagement response)
			{
				ImageMessage imageMessage = ImageMessage.Create(response);
				if (imageMessage != null)
				{
					Debug.Log("Engage returned a valid image message.");
					imageMessage.OnDidReceiveResources += delegate
					{
						Debug.Log("Image Message loaded resources.");
						imageMessage.Show();
					};
					imageMessage.OnDismiss += delegate(ImageMessage.EventArgs obj)
					{
						Debug.Log("Image Message dismissed by " + obj.ID);
					};
					imageMessage.OnAction += delegate(ImageMessage.EventArgs obj)
					{
						Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);
					};
					imageMessage.FetchResources();
				}
				else
				{
					Debug.Log("Engage didn't return an image message.");
				}
			}, delegate(Exception exception)
			{
				Debug.Log("Engage reported an error: " + exception.Message);
			});
		}

		public void OnNotificationOpenedBtn_Clicked()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("_ddId", 1);
			dictionary.Add("_ddName", "Example Notification");
			dictionary.Add("_ddLaunch", true);
			Singleton<DDNA>.Instance.RecordPushNotification(dictionary);
		}

		public void OnUploadEventsBtn_Clicked()
		{
			Singleton<DDNA>.Instance.Upload();
		}

		public void OnStartSDKBtn_Clicked()
		{
			Singleton<DDNA>.Instance.StartSDK("76410301326725846610230818914037", "http://collect2470ntysd.deltadna.net/collect/api", "http://engage2470ntysd.deltadna.net");
		}

		public void OnStopSDKBtn_Clicked()
		{
			Singleton<DDNA>.Instance.StopSDK();
		}

		public void OnNewSessionBtn_Clicked()
		{
			Singleton<DDNA>.Instance.NewSession();
		}
	}
}
