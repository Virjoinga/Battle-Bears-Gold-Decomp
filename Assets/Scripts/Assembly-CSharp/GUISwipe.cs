using UnityEngine;

public class GUISwipe : GUIComponent, InputGUIComponent
{
	private float _maxMoveDist = 100f;

	protected bool _hasTouch;

	protected int _activeTouchID;

	private float _touchX;

	private float _touchY;

	private float _lastTouchX;

	private float _lastTouchY;

	public bool IsHeld { get; private set; }

	public Vector2 SwipeShift { get; protected set; }

	public float InputDepth { get; set; }

	public virtual void AddTo(PlayerGUI gui)
	{
		gui.AddInputComponent(InputDepth, this);
	}

	public virtual void RemoveFrom(PlayerGUI gui)
	{
		gui.RemoveInputComponent(this);
	}

	public void WipeInput()
	{
		_hasTouch = false;
	}

	public bool ClaimsInput(SVTouch touch)
	{
		return _activeTouchID == touch.fingerId;
	}

	public virtual bool ConsumeInput(SVTouch touch)
	{
		if (!_hasTouch)
		{
			if (!IsHeld)
			{
				_lastTouchX = (_touchX = touch.position.x);
				_lastTouchY = (_touchY = touch.position.y);
			}
			else
			{
				_touchX = touch.position.x;
				_touchY = touch.position.y;
			}
			_activeTouchID = touch.fingerId;
			_hasTouch = true;
			return true;
		}
		return false;
	}

	public virtual void FinalizeInput()
	{
		if (_hasTouch)
		{
			IsHeld = true;
			float num = _touchX - _lastTouchX;
			float num2 = _touchY - _lastTouchY;
			if (Mathf.Abs(num) < _maxMoveDist && Mathf.Abs(num2) < _maxMoveDist)
			{
				SwipeShift = new Vector2(num, num2);
			}
			else
			{
				SwipeShift = Vector2.zero;
			}
		}
		else
		{
			_activeTouchID = -1;
			SwipeShift = Vector2.zero;
			IsHeld = false;
		}
		_lastTouchX = _touchX;
		_lastTouchY = _touchY;
	}
}
