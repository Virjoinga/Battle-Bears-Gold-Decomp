using System.Collections.Generic;

public class RadarTrackerManager
{
	private static RadarTrackerManager _instance;

	private List<RadarTracker> _trackers = new List<RadarTracker>();

	public static RadarTrackerManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RadarTrackerManager();
			}
			return _instance;
		}
	}

	public List<RadarTracker> Trackers
	{
		get
		{
			for (int i = 0; i < _trackers.Count; i++)
			{
				if (_trackers[i] == null || _trackers[i].gameObject == null)
				{
					_trackers.RemoveAt(i);
					i--;
				}
			}
			return _trackers;
		}
	}

	public void AddRadarTracker(RadarTracker tracker)
	{
		_trackers.Add(tracker);
	}

	public void RemoveRadarTracker(RadarTracker tracker)
	{
		_trackers.Remove(tracker);
	}
}
