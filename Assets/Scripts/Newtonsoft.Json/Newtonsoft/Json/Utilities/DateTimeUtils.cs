using System;
using System.Globalization;
using System.Xml;

namespace Newtonsoft.Json.Utilities
{
	internal static class DateTimeUtils
	{
		public static string GetUtcOffsetText(this DateTime d)
		{
			TimeSpan utcOffset = d.GetUtcOffset();
			return utcOffset.Hours.ToString("+00;-00", CultureInfo.InvariantCulture) + ":" + utcOffset.Minutes.ToString("00;00", CultureInfo.InvariantCulture);
		}

		public static TimeSpan GetUtcOffset(this DateTime d)
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(d);
		}

		public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
		{
			switch (kind)
			{
			case DateTimeKind.Local:
				return XmlDateTimeSerializationMode.Local;
			case DateTimeKind.Unspecified:
				return XmlDateTimeSerializationMode.Unspecified;
			case DateTimeKind.Utc:
				return XmlDateTimeSerializationMode.Utc;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value.");
			}
		}
	}
}
