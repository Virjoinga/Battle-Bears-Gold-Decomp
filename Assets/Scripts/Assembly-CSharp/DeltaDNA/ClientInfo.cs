using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DeltaDNA
{
	internal static class ClientInfo
	{
		private static string platform;

		private static string deviceName;

		private static string deviceModel;

		private static string deviceType;

		private static string operatingSystem;

		private static string operatingSystemVersion;

		private static string manufacturer;

		private static string timezoneOffset;

		private static string countryCode;

		private static string languageCode;

		private static string locale;

		public static string Platform
		{
			get
			{
				return platform ?? (platform = GetPlatform());
			}
		}

		public static string DeviceName
		{
			get
			{
				return deviceName ?? (deviceName = GetDeviceName());
			}
		}

		public static string DeviceModel
		{
			get
			{
				return deviceModel ?? (deviceModel = GetDeviceModel());
			}
		}

		public static string DeviceType
		{
			get
			{
				return deviceType ?? (deviceType = GetDeviceType());
			}
		}

		public static string OperatingSystem
		{
			get
			{
				return operatingSystem ?? (operatingSystem = GetOperatingSystem());
			}
		}

		public static string OperatingSystemVersion
		{
			get
			{
				return operatingSystemVersion ?? (operatingSystemVersion = GetOperatingSystemVersion());
			}
		}

		public static string Manufacturer
		{
			get
			{
				return manufacturer ?? (manufacturer = GetManufacturer());
			}
		}

		public static string TimezoneOffset
		{
			get
			{
				return timezoneOffset ?? (timezoneOffset = GetCurrentTimezoneOffset());
			}
		}

		public static string CountryCode
		{
			get
			{
				return countryCode ?? (countryCode = GetCountryCode());
			}
		}

		public static string LanguageCode
		{
			get
			{
				return languageCode ?? (languageCode = GetLanguageCode());
			}
		}

		public static string Locale
		{
			get
			{
				return locale ?? (locale = GetLocale());
			}
		}

		private static bool RuntimePlatformIs(string platformName)
		{
			return Enum.IsDefined(typeof(RuntimePlatform), platformName) && Application.platform.ToString() == platformName;
		}

		private static float ScreenSizeInches()
		{
			float num = (float)Screen.width / Screen.dpi;
			float num2 = (float)Screen.height / Screen.dpi;
			return (float)Math.Sqrt(num * num + num2 * num2);
		}

		private static bool IsTablet()
		{
			return ScreenSizeInches() > 6f;
		}

		private static string GetPlatform()
		{
			if (RuntimePlatformIs("OSXEditor") || RuntimePlatformIs("OSXPlayer"))
			{
				return "MAC_CLIENT";
			}
			if (RuntimePlatformIs("WindowsEditor") || RuntimePlatformIs("WindowsPlayer"))
			{
				return "PC_CLIENT";
			}
			if (RuntimePlatformIs("OSXWebPlayer"))
			{
				return "WEB";
			}
			if (RuntimePlatformIs("OSXDashboardPlayer"))
			{
				return "MAC_CLIENT";
			}
			if (RuntimePlatformIs("WindowsWebPlayer"))
			{
				return "WEB";
			}
			if (RuntimePlatformIs("IPhonePlayer"))
			{
				return "IOS";
			}
			if (RuntimePlatformIs("PS3"))
			{
				return "PS3";
			}
			if (RuntimePlatformIs("XBOX360"))
			{
				return "XBOX360";
			}
			if (RuntimePlatformIs("Android"))
			{
				return "ANDROID";
			}
			if (RuntimePlatformIs("NaCL"))
			{
				return "WEB";
			}
			if (RuntimePlatformIs("LinuxEditor") || RuntimePlatformIs("LinuxPlayer"))
			{
				return "PC_CLIENT";
			}
			if (RuntimePlatformIs("WebGLPlayer"))
			{
				return "WEB";
			}
			if (RuntimePlatformIs("FlashPlayer"))
			{
				return "WEB";
			}
			if (RuntimePlatformIs("MetroPlayerX86") || RuntimePlatformIs("MetroPlayerX64") || RuntimePlatformIs("MetroPlayerARM") || RuntimePlatformIs("WSAPlayerX86") || RuntimePlatformIs("WSAPlayerX64") || RuntimePlatformIs("WSAPlayerARM"))
			{
				if (SystemInfo.deviceType == UnityEngine.DeviceType.Handheld)
				{
					return "WINDOWS_MOBILE";
				}
				return "PC_CLIENT";
			}
			if (RuntimePlatformIs("WP8Player"))
			{
				return "WINDOWS_MOBILE";
			}
			if (RuntimePlatformIs("BB10Player") || RuntimePlatformIs("BlackBerryPlayer"))
			{
				return "BLACKBERRY";
			}
			if (RuntimePlatformIs("TizenPlayer"))
			{
				return "ANDROID";
			}
			if (RuntimePlatformIs("PSP2"))
			{
				return "PSVITA";
			}
			if (RuntimePlatformIs("PS4"))
			{
				return "PS4";
			}
			if (RuntimePlatformIs("PSMPlayer"))
			{
				return "WEB";
			}
			if (RuntimePlatformIs("XboxOne"))
			{
				return "XBOXONE";
			}
			if (RuntimePlatformIs("SamsungTVPlayer"))
			{
				return "ANDROID";
			}
			if (RuntimePlatformIs("tvOS"))
			{
				return "IOS_TV";
			}
			if (RuntimePlatformIs("WiiU"))
			{
				return "WIIU";
			}
			if (RuntimePlatformIs("Switch"))
			{
				return "SWITCH";
			}
			return "UNKNOWN";
		}

		private static string GetDeviceName()
		{
			string text = SystemInfo.deviceModel;
			switch (text)
			{
			case "iPhone1,1":
				return "iPhone 1G";
			case "iPhone1,2":
				return "iPhone 3G";
			case "iPhone2,1":
				return "iPhone 3GS";
			case "iPhone3,1":
				return "iPhone 4";
			case "iPhone3,2":
				return "iPhone 4";
			case "iPhone3,3":
				return "iPhone 4";
			case "iPhone4,1":
				return "iPhone 4S";
			case "iPhone5,1":
				return "iPhone 5";
			case "iPhone5,2":
				return "iPhone 5";
			case "iPhone5,3":
				return "iPhone 5C";
			case "iPhone5,4":
				return "iPhone 5C";
			case "iPhone6,1":
				return "iPhone 5S";
			case "iPhone6,2":
				return "iPhone 5S";
			case "iPhone7,2":
				return "iPhone 6";
			case "iPhone7,1":
				return "iPhone 6 Plus";
			case "iPhone8,1":
				return "iPhone 6s";
			case "iPhone8,2":
				return "iPhone 6s Plus";
			case "iPhone8,4":
				return "iPhone SE";
			case "iPhone9,1":
				return "iPhone 7";
			case "iPhone9,2":
				return "iPhone 7 Plus";
			case "iPhone9,3":
				return "iPhone 7";
			case "iPhone9,4":
				return "iPhone 7 Plus";
			case "iPhone10,1":
				return "iPhone 8";
			case "iPhone10,4":
				return "iPhone 8";
			case "iPhone10,2":
				return "iPhone 8 Plus";
			case "iPhone10,5":
				return "iPhone 8 Plus";
			case "iPhone10,3":
				return "iPhone X";
			case "iPhone10,6":
				return "iPhone X";
			case "iPod1,1":
				return "iPod Touch 1G";
			case "iPod2,1":
				return "iPod Touch 2G";
			case "iPod3,1":
				return "iPod Touch 3G";
			case "iPod4,1":
				return "iPod Touch 4G";
			case "iPod5,1":
				return "iPod Touch 5G";
			case "iPod7,1":
				return "iPod Touch 6G";
			case "iPad1,1":
				return "iPad 1G";
			case "iPad2,1":
				return "iPad 2";
			case "iPad2,2":
				return "iPad 2";
			case "iPad2,3":
				return "iPad 2";
			case "iPad2,4":
				return "iPad 2";
			case "iPad2,5":
				return "iPad Mini 1G";
			case "iPad2,6":
				return "iPad Mini 1G";
			case "iPad2,7":
				return "iPad Mini 1G";
			case "iPad3,1":
				return "iPad 3G";
			case "iPad3,2":
				return "iPad 3G";
			case "iPad3,3":
				return "iPad 3G";
			case "iPad3,4":
				return "iPad 4G";
			case "iPad3,5":
				return "iPad 4G";
			case "iPad3,6":
				return "iPad 4G";
			case "iPad4,1":
				return "iPad Air";
			case "iPad4,2":
				return "iPad Air";
			case "iPad4,3":
				return "iPad Air";
			case "iPad4,4":
				return "iPad Mini 2G";
			case "iPad4,5":
				return "iPad Mini 2G";
			case "iPad4,6":
				return "iPad Mini 2G";
			case "iPad4,7":
				return "iPad Mini 3";
			case "iPad4,8":
				return "iPad Mini 3";
			case "iPad4,9":
				return "iPad Mini 3";
			case "iPad5,1":
				return "iPad Mini 4";
			case "iPad5,2":
				return "iPad Mini 4";
			case "iPad5,3":
				return "iPad Air 2";
			case "iPad5,4":
				return "iPad Air 2";
			case "iPad6,7":
				return "iPad Pro 12.9";
			case "iPad6,8":
				return "iPad Pro 12.9";
			case "iPad6,3":
				return "iPad Pro 9.7";
			case "iPad6,4":
				return "iPad Pro 9.7";
			case "iPad6,11":
				return "iPad 5G";
			case "iPad6,12":
				return "iPad 5G";
			case "iPad7,1":
				return "iPad Pro 12.9 2G";
			case "iPad7,2":
				return "iPad Pro 12.9 2G";
			case "iPad7,3":
				return "iPad Pro 10.5";
			case "iPad7,4":
				return "iPad Pro 10.5";
			case "Amazon KFSAWA":
				return "Fire HDX 8.9 (4th Gen)";
			case "Amazon KFASWI":
				return "Fire HD 7 (4th Gen)";
			case "Amazon KFARWI":
				return "Fire HD 6 (4th Gen)";
			case "Amazon KFAPWA":
			case "Amazon KFAPWI":
				return "Kindle Fire HDX 8.9 (3rd Gen)";
			case "Amazon KFTHWA":
			case "Amazon KFTHWI":
				return "Kindle Fire HDX 7 (3rd Gen)";
			case "Amazon KFSOWI":
				return "Kindle Fire HD 7 (3rd Gen)";
			case "Amazon KFJWA":
			case "Amazon KFJWI":
				return "Kindle Fire HD 8.9 (2nd Gen)";
			case "Amazon KFTT":
				return "Kindle Fire HD 7 (2nd Gen)";
			case "Amazon KFOT":
				return "Kindle Fire (2nd Gen)";
			case "Amazon Kindle Fire":
				return "Kindle Fire (1st Gen)";
			case "Amazon KFGIWI":
				return "Fire HD 8 (2016)";
			case "Amazon KFDOWI":
				return "Fire HD 8 (2017)";
			case "Amazon KFAUWI":
				return "Fire 7 (2017)";
			case "Amazon KFSUWI":
				return "Fire HD 10 (2017)";
			default:
				return Trim(text, 72);
			}
		}

		private static string GetDeviceModel()
		{
			return Trim(SystemInfo.deviceModel, 72);
		}

		private static string GetDeviceType()
		{
			if (RuntimePlatformIs("SamsungTVPlayer"))
			{
				return "TV";
			}
			if (RuntimePlatformIs("tvOS"))
			{
				return "TV";
			}
			switch (SystemInfo.deviceType)
			{
			case UnityEngine.DeviceType.Console:
				return "CONSOLE";
			case UnityEngine.DeviceType.Desktop:
				return "PC";
			case UnityEngine.DeviceType.Handheld:
			{
				string text = SystemInfo.deviceModel;
				if (text.StartsWith("iPhone"))
				{
					return "MOBILE_PHONE";
				}
				if (text.StartsWith("iPad"))
				{
					return "TABLET";
				}
				return (!IsTablet()) ? "MOBILE_PHONE" : "TABLET";
			}
			default:
				return "UNKNOWN";
			}
		}

		private static string GetOperatingSystem()
		{
			if (RuntimePlatformIs("tvOS"))
			{
				return "TVOS";
			}
			string text = SystemInfo.operatingSystem.ToUpper();
			if (text.Contains("WINDOWS"))
			{
				return "WINDOWS";
			}
			if (text.Contains("OSX"))
			{
				return "OSX";
			}
			if (text.Contains("MAC"))
			{
				return "OSX";
			}
			if (text.Contains("IOS") || text.Contains("IPHONE") || text.Contains("IPAD"))
			{
				return "IOS";
			}
			if (text.Contains("LINUX"))
			{
				return "LINUX";
			}
			if (text.Contains("ANDROID"))
			{
				if (SystemInfo.deviceModel.ToUpper().Contains("AMAZON"))
				{
					return "FIREOS";
				}
				return "ANDROID";
			}
			if (text.Contains("BLACKBERRY"))
			{
				return "BLACKBERRY";
			}
			return "UNKNOWN";
		}

		private static string GetOperatingSystemVersion()
		{
			try
			{
				Regex regex = new Regex("[\\d|\\.]+");
				string input = SystemInfo.operatingSystem;
				Match match = regex.Match(input);
				if (match.Success)
				{
					return match.Groups[0].ToString();
				}
				return string.Empty;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static string GetManufacturer()
		{
			return Trim(new AndroidJavaObject("android.os.Build").GetStatic<string>("MANUFACTURER"), 72);
		}

		private static string GetCurrentTimezoneOffset()
		{
			DateTime now = DateTime.Now;
			TimeSpan timeSpan = TimeSpan.Zero;
			bool flag = false;
			try
			{
				if (TimeZoneInfo.Local != null)
				{
					timeSpan = TimeZoneInfo.Local.GetUtcOffset(now);
					flag = true;
				}
			}
			catch (TimeZoneNotFoundException)
			{
			}
			catch (NullReferenceException)
			{
			}
			if (!flag)
			{
				try
				{
					if (TimeZone.CurrentTimeZone != null)
					{
						timeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(now);
						flag = true;
					}
				}
				catch (TimeZoneNotFoundException)
				{
				}
			}
			if (!flag)
			{
				Debug.LogWarning("Failed to retrieve timezone offset");
			}
			return string.Format("{0}{1:D2}", (timeSpan.Hours < 0) ? string.Empty : "+", timeSpan.Hours);
		}

		private static string GetCountryCode()
		{
			return null;
		}

		private static string GetLanguageCode()
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.Afrikaans:
				return "af";
			case SystemLanguage.Arabic:
				return "ar";
			case SystemLanguage.Basque:
				return "eu";
			case SystemLanguage.Belarusian:
				return "be";
			case SystemLanguage.Bulgarian:
				return "bg";
			case SystemLanguage.Catalan:
				return "ca";
			case SystemLanguage.Chinese:
				return "zh";
			case SystemLanguage.Czech:
				return "cs";
			case SystemLanguage.Danish:
				return "da";
			case SystemLanguage.Dutch:
				return "nl";
			case SystemLanguage.English:
				return "en";
			case SystemLanguage.Estonian:
				return "et";
			case SystemLanguage.Faroese:
				return "fo";
			case SystemLanguage.Finnish:
				return "fi";
			case SystemLanguage.French:
				return "fr";
			case SystemLanguage.German:
				return "de";
			case SystemLanguage.Greek:
				return "el";
			case SystemLanguage.Hebrew:
				return "he";
			case SystemLanguage.Hungarian:
				return "hu";
			case SystemLanguage.Icelandic:
				return "is";
			case SystemLanguage.Indonesian:
				return "id";
			case SystemLanguage.Italian:
				return "it";
			case SystemLanguage.Japanese:
				return "ja";
			case SystemLanguage.Korean:
				return "ko";
			case SystemLanguage.Latvian:
				return "lv";
			case SystemLanguage.Lithuanian:
				return "lt";
			case SystemLanguage.Norwegian:
				return "nn";
			case SystemLanguage.Polish:
				return "pl";
			case SystemLanguage.Portuguese:
				return "pt";
			case SystemLanguage.Romanian:
				return "ro";
			case SystemLanguage.Russian:
				return "ru";
			case SystemLanguage.SerboCroatian:
				return "sr";
			case SystemLanguage.Slovak:
				return "sk";
			case SystemLanguage.Slovenian:
				return "sl";
			case SystemLanguage.Spanish:
				return "es";
			case SystemLanguage.Swedish:
				return "sv";
			case SystemLanguage.Thai:
				return "th";
			case SystemLanguage.Turkish:
				return "tr";
			case SystemLanguage.Ukrainian:
				return "uk";
			case SystemLanguage.Vietnamese:
				return "vi";
			default:
				return "en";
			}
		}

		private static string GetLocale()
		{
			if (CountryCode != null)
			{
				return string.Format("{0}_{1}", LanguageCode, CountryCode);
			}
			return string.Format("{0}_ZZ", LanguageCode);
		}

		private static string Trim(string value, int length)
		{
			if (value == null)
			{
				return null;
			}
			return value.Substring(0, Math.Min(value.Length, length));
		}
	}
}
