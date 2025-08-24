using System;

namespace Prime31.MetroEssentials
{
	public static class Toasts
	{
		public static void scheduleToast(ToastTemplateType toastTemplateType, string[] text, DateTimeOffset deliveryTime)
		{
		}

		public static void scheduleToast(ToastTemplateType toastTemplateType, string[] text, string image, DateTimeOffset deliveryTime)
		{
		}

		public static void showToast(ToastTemplateType toastTemplateType, string[] text)
		{
		}

		public static void showToast(ToastTemplateType toastTemplateType, string[] text, string image)
		{
		}

		public static void showToast(ToastTemplateType toastTemplateType, string[] text, string image, DateTimeOffset? expirationTime)
		{
		}

		public static void showToast(ToastTemplateType toastTemplateType, string[] text, string image, DateTimeOffset? expirationTime, Action<string> dismissedHandler, Action activatedHandler, Action<Exception> failedHandler)
		{
		}
	}
}
