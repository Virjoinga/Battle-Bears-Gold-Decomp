using System.Collections.Generic;
using Analytics.Parameters;
using Analytics.Schemas;
using UnityEngine;

namespace Analytics
{
	public static class EventTracker
	{
		private static IEventTracker _eventTracker = new DeltaDNAEventTracker();

		private static EventSchema _lastEvent;

		private static bool _hasInitialized = false;

		private static List<EventSchema> _pendingEvents = new List<EventSchema>();

		public static void Init(string userId)
		{
			_eventTracker.EnableTracking(userId);
			_hasInitialized = true;
			foreach (EventSchema pendingEvent in _pendingEvents)
			{
				TrackEvent(pendingEvent);
			}
			_pendingEvents.Clear();
		}

		public static void ForceSendEvents()
		{
			_eventTracker.ForceSendEvents();
		}

		public static void TrackEvent(EventSchema schema)
		{
			if (_hasInitialized)
			{
				_eventTracker.TrackEvent(schema.Name.ToString(), schema.ToDictionary());
				if (schema.Name != 0)
				{
					_lastEvent = schema;
				}
			}
			else
			{
				_pendingEvents.Add(schema);
			}
		}

		public static void TrackLastEvent()
		{
			if (_lastEvent != null)
			{
				TrackEvent(new UserLeftAppSchema(new SessionTimePlayedParameter(Mathf.RoundToInt(Bootloader.Instance.SessionTime)), new LastEventSentParameter(_lastEvent.Name)));
			}
		}
	}
}
