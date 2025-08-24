using System.Collections.Generic;

public interface IEventTracker
{
	void EnableTracking(string userId = null);

	void TrackEvent(string name, IDictionary<string, object> parameters);

	void TrackEvent(string name);

	void ForceSendEvents();
}
