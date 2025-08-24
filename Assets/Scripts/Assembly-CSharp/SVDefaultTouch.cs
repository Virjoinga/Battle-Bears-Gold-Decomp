using UnityEngine;

public class SVDefaultTouch : SVTouch
{
	public SVDefaultTouch(Touch touch)
	{
		SetTo(touch);
	}

	public void SetTo(Touch touch)
	{
		base.fingerId = touch.fingerId;
		base.position = touch.position;
		base.deltaPosition = touch.deltaPosition;
		base.deltaTime = touch.deltaTime;
		base.tapCount = touch.tapCount;
		base.phase = touch.phase;
	}
}
