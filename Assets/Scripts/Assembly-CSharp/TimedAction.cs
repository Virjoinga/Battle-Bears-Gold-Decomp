using UnityEngine;

public class TimedAction : MonoBehaviour
{
	public float duration = 10f;

	public float appearEverySeconds = 20f;

	private bool _on;

	private bool _firstTick = true;

	private int _previousOnTime;

	private void Start()
	{
		base.GetComponent<Renderer>().enabled = false;
		base.GetComponent<Collider>().enabled = false;
		base.GetComponent<Light>().enabled = false;
		if (PhotonManager.Instance != null)
		{
			int startTime = GameManager.Instance.StartTime;
			int num = startTime % (int)(appearEverySeconds * 1000f);
			_previousOnTime = startTime - num;
		}
	}

	private void Update()
	{
		if (PhotonManager.Instance != null)
		{
			int serverTimeInMilliseconds = PhotonManager.Instance.ServerTimeInMilliseconds;
			if (_firstTick && serverTimeInMilliseconds != 0)
			{
				_previousOnTime = serverTimeInMilliseconds;
				_firstTick = false;
			}
			if (_on)
			{
				if ((float)(serverTimeInMilliseconds - _previousOnTime) > duration * 1000f)
				{
					_on = false;
				}
			}
			else if ((float)(serverTimeInMilliseconds - _previousOnTime) > appearEverySeconds * 1000f)
			{
				_previousOnTime = serverTimeInMilliseconds;
				_on = true;
			}
		}
		base.GetComponent<Renderer>().enabled = _on;
		base.GetComponent<Collider>().enabled = _on;
		base.GetComponent<Light>().enabled = _on;
	}
}
