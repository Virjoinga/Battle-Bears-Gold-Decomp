using UnityEngine;

public class GUIJoystick : GUIComponent, InputGUIComponent, RenderedGUIComponent
{
	protected Rect _baseRect;

	protected Rect _nubRect;

	protected bool _hasTouch;

	protected int _activeTouchID;

	protected float _touchX;

	protected float _touchY;

	protected float _lastTouchX;

	protected float _lastTouchY;

	protected float _diffX;

	protected float _diffY;

	public bool Enabled { get; set; }

	public bool IsHeld { get; protected set; }

	public Vector2 InputVector { get; protected set; }

	public bool OnDoubleTap { get; protected set; }

	public Texture2D BaseTexture { get; set; }

	public Texture2D NubTexture { get; set; }

	public float PadX { get; set; }

	public float PadY { get; set; }

	public float PadRadius { get; set; }

	public float NubRadius { get; set; }

	public float RenderDepth { get; set; }

	public float InputDepth { get; set; }

	public void AddTo(PlayerGUI gui)
	{
		Enabled = true;
		gui.AddInputComponent(InputDepth, this);
		gui.AddRenderedComponent(RenderDepth, this);
	}

	public void RemoveFrom(PlayerGUI gui)
	{
		gui.RemoveInputComponent(this);
		gui.RemoveRenderedComponent(this);
	}

	public void WipeInput()
	{
		_hasTouch = false;
	}

	public void ResetTouchID()
	{
		_activeTouchID = -1;
	}

	public bool ClaimsInput(SVTouch touch)
	{
		return _activeTouchID == touch.fingerId && Enabled;
	}

	public virtual bool ConsumeInput(SVTouch touch)
	{
		if (!Enabled)
		{
			return false;
		}
		if (!_hasTouch && (_activeTouchID == -1 || touch.fingerId == _activeTouchID))
		{
			_touchX = touch.position.x;
			_touchY = (float)Screen.height - touch.position.y;
			_diffX = _touchX - PadX;
			_diffY = PadY - _touchY;
			if (touch.fingerId == _activeTouchID)
			{
				float num = _touchX - _lastTouchX;
				float num2 = _touchY - _lastTouchY;
				if (Mathf.Sqrt(num * num + num2 * num2) <= 300f)
				{
					_hasTouch = true;
					_activeTouchID = touch.fingerId;
					if (touch.tapCount > 1)
					{
						OnDoubleTap = true;
					}
					return true;
				}
			}
			else if (Mathf.Sqrt(_diffX * _diffX + _diffY * _diffY) <= PadRadius)
			{
				_hasTouch = true;
				_activeTouchID = touch.fingerId;
				if (touch.tapCount > 1)
				{
					OnDoubleTap = true;
				}
				return true;
			}
		}
		return false;
	}

	public virtual void FinalizeInput()
	{
		if (_hasTouch)
		{
			IsHeld = true;
			InputVector = new Vector2(_diffX / PadRadius, _diffY / PadRadius);
			if (InputVector.magnitude > 1f)
			{
				InputVector = InputVector.normalized;
			}
			_lastTouchX = _touchX;
			_lastTouchY = _touchY;
		}
		else
		{
			_touchX = PadX;
			_touchY = PadY;
			_diffX = 0f;
			_diffY = 0f;
			_activeTouchID = -1;
			IsHeld = false;
			InputVector = Vector2.zero;
			OnDoubleTap = false;
			_lastTouchX = PadX;
			_lastTouchY = PadY;
		}
		CalculateDrawRects();
	}

	protected virtual void CalculateDrawRects()
	{
		_baseRect = new Rect(PadX - PadRadius, PadY - PadRadius, PadRadius * 2f, PadRadius * 2f);
		_nubRect = new Rect(_touchX - NubRadius, _touchY - NubRadius, NubRadius * 2f, NubRadius * 2f);
	}

	public void RenderGUI()
	{
		if (Enabled)
		{
			if (BaseTexture != null)
			{
				GUI.color = ((!IsHeld) ? Preferences.Instance.HUDUnPressedButtonColor : Preferences.Instance.HUDPressedButtonColor);
				GUI.DrawTexture(_baseRect, BaseTexture);
			}
			if (NubTexture != null)
			{
				GUI.color = ((!IsHeld) ? Preferences.Instance.HUDUnPressedButtonColor : Preferences.Instance.HUDPressedButtonColor);
				GUI.DrawTexture(_nubRect, NubTexture);
			}
		}
	}
}
