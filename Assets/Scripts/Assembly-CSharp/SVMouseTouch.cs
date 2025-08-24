using UnityEngine;

public class SVMouseTouch : SVTouch
{
	private float _timeSinceLastClick;

	private int _numClicks;

	public bool active { get; private set; }

	public SVMouseTouch()
	{
		base.fingerId = 10;
		base.position = Vector2.zero;
		base.deltaPosition = Vector2.zero;
		base.deltaTime = 0f;
		base.tapCount = 0;
		base.phase = TouchPhase.Ended;
		active = false;
	}

	public void Update(float delta)
	{
		base.deltaTime = delta;
		if (Input.GetMouseButton(0))
		{
			active = true;
			if (base.phase == TouchPhase.Ended)
			{
				base.phase = TouchPhase.Began;
				base.deltaPosition = Vector2.zero;
				base.position = Input.mousePosition;
				_numClicks = ((!(_timeSinceLastClick < 0.5f)) ? 1 : (_numClicks + 1));
				_timeSinceLastClick = 0f;
			}
			else
			{
				Vector2 vector = Input.mousePosition;
				base.deltaPosition = vector - base.position;
				base.position = vector;
				base.phase = ((base.deltaPosition.sqrMagnitude > 0f) ? TouchPhase.Moved : TouchPhase.Stationary);
			}
		}
		else
		{
			active &= base.phase != TouchPhase.Ended;
			Vector2 vector2 = Input.mousePosition;
			base.deltaPosition = vector2 - base.position;
			base.position = vector2;
			base.phase = TouchPhase.Ended;
		}
		_timeSinceLastClick += delta;
	}
}
