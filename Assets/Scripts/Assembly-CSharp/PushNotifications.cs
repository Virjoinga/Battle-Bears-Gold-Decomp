using DeltaDNA;
using UnityEngine;

public static class PushNotifications
{
	public static void RegisterForPushNotifications()
	{
		Debug.Log("Registering for push notifications");
		Singleton<DDNA>.Instance.AndroidNotifications.UnregisterForPushNotifications();
		Singleton<DDNA>.Instance.AndroidNotifications.RegisterForPushNotifications();
	}
}
