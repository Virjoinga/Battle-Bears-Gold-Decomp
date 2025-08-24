using UnityEngine;

public class GUIDoubleTapJoystickArea : GUIComponent, InputGUIComponent
{
	private static readonly string JOYSTICK_BASE_TEXTURE_NAME = "Textures/GUI/joystickBase";

	private static readonly string JOYSTICK_NUB_TEXTURE_NAME = "Textures/GUI/joystickNub";

	private float _maxMoveDist = 100f;

	private float _maxDeltaX = 10f;

	protected bool _hasTouch;

	protected int _activeTouchID;

	private float _touchX;

	private float _touchY;

	private float _lastTouchX;

	private float _lastTouchY;

	public bool IsHeld { get; private set; }

	public bool DoubleTapped { get; private set; }

	public Vector2 JoystickAreaDirection { get; protected set; }

	public float InputDepth { get; set; }

	public float RenderDepth { get; set; }

	public void AddTo(PlayerGUI gui)
	{
		gui.AddInputComponent(InputDepth, this);
	}

	public void RemoveFrom(PlayerGUI gui)
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

	public bool ConsumeInput(SVTouch touch)
	{
		bool result = false;
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
			if (touch.tapCount > 1)
			{
				DoubleTapped = true;
			}
			result = true;
		}
		return result;
	}

	public void FinalizeInput()
	{
		if (_hasTouch)
		{
			IsHeld = true;
			float num = _touchX - _lastTouchX;
			float y = _touchY - _lastTouchY;
			num /= 10f;
			if (Mathf.Abs(num) > _maxDeltaX)
			{
				num = ((!(num < 0f)) ? _maxDeltaX : (0f - _maxDeltaX));
			}
			Vector2 joystickAreaDirection = new Vector2(num, 0f);
			joystickAreaDirection.y = y;
			JoystickAreaDirection = joystickAreaDirection;
			_lastTouchY = _touchY;
		}
		else
		{
			_activeTouchID = -1;
			JoystickAreaDirection = Vector2.zero;
			IsHeld = false;
			DoubleTapped = false;
		}
	}
}
