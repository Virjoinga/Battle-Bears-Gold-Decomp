using System;

namespace Prime31.MetroEssentials
{
	public static class Tiles
	{
		public static void startPeriodicUpdate(string url, PeriodicUpdateRecurrence periodicUpdateRecurrenceType)
		{
		}

		public static void startPeriodicUpdate(string url, PeriodicUpdateRecurrence periodicUpdateRecurrenceType, DateTime? startTime)
		{
		}

		public static void stopPeriodicUpdate()
		{
		}

		public static void enableNotificationQueue(bool enable)
		{
		}

		public static void updateTile(string xml, DateTimeOffset? expirationTime)
		{
		}

		public static void updateTile(TileTemplateType tileTemplateType, string[] text)
		{
		}

		public static void updateTile(TileTemplateType tileTemplateType, string[] text, string[] images)
		{
		}

		public static void updateTile(TileTemplateType tileTemplateType, string[] text, string[] images, DateTimeOffset? expirationTime)
		{
		}

		public static string getTemplateContent(TileTemplateType tileTemplateType)
		{
			return null;
		}

		public static void clearTile()
		{
		}

		public static void updateBadge(string value)
		{
		}

		public static void clearBadge()
		{
		}
	}
}
