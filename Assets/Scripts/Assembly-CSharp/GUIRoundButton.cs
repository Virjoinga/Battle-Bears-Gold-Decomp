using UnityEngine;

public class GUIRoundButton : GUIComponent, InputGUIComponent, RenderedGUIComponent
{
	protected Rect _backgroundDrawRect;

	protected Rect _iconDrawRect;

	private float _iconScale = 0.75f;

	private bool _hasTouch;

	protected int _activeTouchID;

	public bool WasPressed { get; private set; }

	public bool IsHeld { get; private set; }

	public bool OnDoubleTap { get; private set; }

	public Texture2D BackgroundTexture { get; set; }

	public Texture2D Icon { get; set; }

	public float ButtonX { get; set; }

	public float ButtonY { get; set; }

	public float ButtonRadius { get; set; }

	public float RenderDepth { get; set; }

	public float InputDepth { get; set; }

	public bool OverrideColorPref { get; set; }

	public Color ColorOverride { get; set; }

	public void AddTo(PlayerGUI gui)
	{
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

	public virtual bool ClaimsInput(SVTouch touch)
	{
		return _activeTouchID == touch.fingerId;
	}

	public virtual bool ConsumeInput(SVTouch touch)
	{
		if (!_hasTouch)
		{
			float x = touch.position.x;
			float num = (float)Screen.height - touch.position.y;
			float num2 = x - ButtonX;
			float num3 = ButtonY - num;
			if (Mathf.Sqrt(num2 * num2 + num3 * num3) <= ButtonRadius)
			{
				_activeTouchID = touch.fingerId;
				_hasTouch = true;
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
			WasPressed = !IsHeld;
			IsHeld = true;
		}
		else
		{
			_activeTouchID = -1;
			IsHeld = false;
			WasPressed = false;
			OnDoubleTap = false;
		}
		_backgroundDrawRect = new Rect(ButtonX - ButtonRadius, ButtonY - ButtonRadius, ButtonRadius * 2f, ButtonRadius * 2f);
		_iconDrawRect = new Rect(ButtonX - ButtonRadius * _iconScale, ButtonY - ButtonRadius * _iconScale, ButtonRadius * 2f * _iconScale, ButtonRadius * 2f * _iconScale);
	}

	public virtual void RenderGUI()
	{
		if (BackgroundTexture != null)
		{
			Color color2 = (GUI.color = ((!IsHeld) ? Preferences.Instance.HUDUnPressedButtonColor : Preferences.Instance.HUDPressedButtonColor));
			GUI.color = color2;
			if (OverrideColorPref)
			{
				GUI.color = ColorOverride;
			}
			GUI.DrawTexture(_backgroundDrawRect, BackgroundTexture);
		}
		if (Icon != null)
		{
			GUI.color = Preferences.Instance.HUDButtonIconColor;
			GUI.DrawTexture(_iconDrawRect, Icon);
		}
	}
}
