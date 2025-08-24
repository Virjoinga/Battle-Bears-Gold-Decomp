using UnityEngine;

namespace DeltaDNA.Android
{
	internal class DDNANotifications
	{
		private AndroidJavaClass ddnaNotifications;

		public DDNANotifications()
		{
			ddnaNotifications = new AndroidJavaClass("com.deltadna.android.sdk.notifications.DDNANotifications");
		}

		public void MarkUnityLoaded()
		{
			ddnaNotifications.CallStatic("markUnityLoaded");
		}

		public void Register(AndroidJavaObject context, bool secondary)
		{
			ddnaNotifications.CallStatic("register", context, secondary);
		}
	}
}
