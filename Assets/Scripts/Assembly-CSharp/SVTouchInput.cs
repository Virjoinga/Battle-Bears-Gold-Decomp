using System.Collections.Generic;
using UnityEngine;

public class SVTouchInput
{
	private static List<SVTouch> _touchList = new List<SVTouch>();

	private static SVMouseTouch _mouseTouch = new SVMouseTouch();

	private static float _lastTime;

	public static SVTouch[] Touches { get; private set; }

	public static void UpdateTouches()
	{
		_touchList.Clear();
        /*Touch[] touches = Input.touches;
		foreach (Touch touch in touches)
		{
			_touchList.Add(new SVDefaultTouch(touch));
		}
		Touches = _touchList.ToArray();*/
		_touchList.Add(_mouseTouch);
        _mouseTouch.Update(Time.unscaledDeltaTime);
		Touches = _touchList.ToArray();
	}
}
