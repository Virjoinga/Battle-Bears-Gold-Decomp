using UnityEngine;

public class GUIRectangleButton : GUIComponent, InputGUIComponent, RenderedGUIComponent
{
	protected Rect _backgroundDrawRect;

	protected Rect _iconDrawRect;

	private float _iconScale = 0.75f;

	private float _iconX;

	private float _iconY;

	private float _iconWidth;

	private float _iconHeight;

	private bool _hasTouch;

	protected int _activeTouchID;

	public bool Enabled { get; set; }

	public bool WasPressed { get; private set; }

	public bool IsHeld { get; private set; }

	public bool OnDoubleTap { get; private set; }

	public Texture2D BackgroundTexture { get; set; }

	public Texture2D Icon { get; set; }

	public Vector2 IconPercentOffset { get; set; }

	public float IconScale
	{
		get
		{
			return _iconScale;
		}
		set
		{
			_iconScale = value;
		}
	}

	public float ButtonX { get; set; }

	public float ButtonY { get; set; }

	public float ButtonWidth { get; set; }

	public float ButtonHeight { get; set; }

	public float RenderDepth { get; set; }

	public float InputDepth { get; set; }

	public void AddTo(PlayerGUI gui)
	{
		gui.AddInputComponent(InputDepth, this);
		gui.AddRenderedComponent(RenderDepth, this);
		Enabled = true;
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
		if (!Enabled)
		{
			return false;
		}
		return _activeTouchID == touch.fingerId;
	}

	public virtual bool ConsumeInput(SVTouch touch)
	{
		if (!Enabled)
		{
			return false;
		}
		if (!_hasTouch)
		{
			float x = touch.position.x;
			float num = (float)Screen.height - touch.position.y;
			if (x >= ButtonX - ButtonWidth / 2f && num >= ButtonY - ButtonHeight / 2f && x <= ButtonX + ButtonWidth / 2f && num <= ButtonY + ButtonHeight / 2f)
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
		if (!Enabled)
		{
			return;
		}
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
		_backgroundDrawRect = new Rect(ButtonX - ButtonWidth / 2f, ButtonY - ButtonHeight / 2f, ButtonWidth, ButtonHeight);
		if (Icon != null)
		{
			if (_iconWidth == 0f || _iconHeight == 0f)
			{
				_iconX = ButtonX + IconPercentOffset.x * (float)BackgroundTexture.width * PlayerGUI.Instance.SmallestRatio;
				_iconY = ButtonY + IconPercentOffset.y * (float)BackgroundTexture.height * PlayerGUI.Instance.SmallestRatio;
				_iconWidth = (float)Icon.width * PlayerGUI.Instance.SmallestRatio * _iconScale;
				_iconHeight = (float)Icon.height * PlayerGUI.Instance.SmallestRatio * _iconScale;
			}
			_iconDrawRect = new Rect(_iconX - _iconWidth / 2f, _iconY - _iconHeight / 2f, _iconWidth, _iconHeight);
		}
	}

	public virtual void RenderGUI()
	{
		if (Enabled)
		{
			if (BackgroundTexture != null)
			{
				Color color2 = (GUI.color = ((!IsHeld) ? Preferences.Instance.HUDUnPressedButtonColor : Preferences.Instance.HUDPressedButtonColor));
				GUI.color = color2;
				GUI.DrawTexture(_backgroundDrawRect, BackgroundTexture);
			}
			if (Icon != null)
			{
				GUI.color = Preferences.Instance.HUDButtonIconColor;
				GUI.DrawTexture(_iconDrawRect, Icon);
			}
		}
	}
}
