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
		base.renderer.enabled = false;
		base.collider.enabled = false;
		base.light.enabled = false;
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
		base.renderer.enabled = _on;
		base.collider.enabled = _on;
		base.light.enabled = _on;
	}
}
