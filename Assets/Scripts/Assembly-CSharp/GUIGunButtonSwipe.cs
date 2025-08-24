using UnityEngine;

public class GUIGunButtonSwipe : GUIJoystick
{
	private float _currentX;

	private float _currentY;

	private float _lastX;

	private float _lastY;

	public Vector2 SwipeDelta { get; set; }

	public override bool ConsumeInput(SVTouch touch)
	{
		bool isHeld = base.IsHeld;
		bool result = base.ConsumeInput(touch);
		if (_activeTouchID == touch.fingerId)
		{
			if (!isHeld)
			{
				_lastX = (_currentX = touch.position.x);
				_lastY = (_currentY = touch.position.y);
			}
			else
			{
				_currentX = touch.position.x;
				_currentY = touch.position.y;
			}
		}
		return result;
	}

	public override void FinalizeInput()
	{
		base.FinalizeInput();
		base.InputVector = Vector2.zero;
		if (_hasTouch)
		{
			SwipeDelta = new Vector2(_currentX - _lastX, _currentY - _lastY);
		}
		else
		{
			SwipeDelta = Vector2.zero;
		}
		_lastX = _currentX;
		_lastY = _currentY;
	}

	protected override void CalculateDrawRects()
	{
		if (Preferences.Instance.CurrentShootMode == ShootMode.shootButton)
		{
			base.CalculateDrawRects();
			return;
		}
		_baseRect = new Rect(base.PadX - base.PadRadius, base.PadY - base.PadRadius, base.PadRadius * 2f, base.PadRadius * 2f);
		_nubRect = new Rect(base.PadX - base.NubRadius, base.PadY - base.NubRadius, base.NubRadius * 2f, base.NubRadius * 2f);
	}
}
