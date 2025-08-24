using System.Collections.Generic;
using DeltaDNA;

public class DeltaDNAEventTracker : IEventTracker
{
	private string ENVIRONMENT_KEY = "81255031153391403088229760015106";

	private string COLLECT_URL = "https://collect12429bttlb.deltadna.net/collect/api";

	private string ENGAGE_URL = "https://engage12429bttlb.deltadna.net";

	private DDNA _tracker
	{
		get
		{
			return Singleton<DDNA>.Instance;
		}
	}

	public void EnableTracking(string userId = null)
	{
		_tracker.ClientVersion = ServiceManager.Instance.ClientVersion;
		_tracker.StartSDK(ENVIRONMENT_KEY, COLLECT_URL, ENGAGE_URL, userId);
	}

	public void ForceSendEvents()
	{
		_tracker.Upload();
	}

	public void TrackEvent(string name, IDictionary<string, object> parameters)
	{
		GameEvent gameEvent = new GameEvent(name);
		if (parameters != null)
		{
			foreach (KeyValuePair<string, object> parameter in parameters)
			{
				gameEvent.AddParam(parameter.Key, parameter.Value);
			}
		}
		_tracker.RecordEvent(gameEvent);
	}

	public void TrackEvent(string name)
	{
		_tracker.RecordEvent(name);
	}
}
