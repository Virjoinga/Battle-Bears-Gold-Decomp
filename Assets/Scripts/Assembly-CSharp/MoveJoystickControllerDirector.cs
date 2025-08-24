using UnityEngine;

public class MoveJoystickControllerDirector : SimpleControllerDirector
{
	protected static readonly float DEADZONE_RADIUS = 0.2f;

	private static readonly string JOYSTICK_BASE_TEXTURE_NAME = "Textures/GUI/joystickBase";

	private static readonly string JOYSTICK_NUB_TEXTURE_NAME = "Textures/GUI/joystickNub";

	protected GUIJoystick _moveJoystick;

	protected GUISwipe _swipe;

	private Texture2D _joystickBase;

	private Texture2D _joystickNub;

	public GUIJoystick MoveJoystick
	{
		get
		{
			return _moveJoystick;
		}
	}

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		_joystickBase = Resources.Load(JOYSTICK_BASE_TEXTURE_NAME) as Texture2D;
		_joystickNub = Resources.Load(JOYSTICK_NUB_TEXTURE_NAME) as Texture2D;
		_moveJoystick = new GUIJoystick();
		_moveJoystick.InputDepth = 10f;
		_moveJoystick.RenderDepth = 10f;
		_moveJoystick.BaseTexture = _joystickBase;
		_moveJoystick.NubTexture = _joystickNub;
		_swipe = new GUISwipe();
		_swipe.InputDepth = 0f;
		PositionGUI();
		player.PlayersGUI.AddComponent(_moveJoystick);
		player.PlayersGUI.AddComponent(_swipe);
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
		player.PlayersGUI.RemoveComponent(_moveJoystick);
		player.PlayersGUI.RemoveComponent(_swipe);
	}

	public override void PositionGUI()
	{
		base.PositionGUI();
		int width = Screen.width;
		int height = Screen.height;
		float padRadius = (float)_moveJoystick.BaseTexture.width * Preferences.Instance.ButtonSize * PlayerGUI.Instance.SmallestRatio;
		float nubRadius = (float)_moveJoystick.NubTexture.width * Preferences.Instance.ButtonSize * PlayerGUI.Instance.SmallestRatio;
		_moveJoystick.PadRadius = padRadius;
		_moveJoystick.PadX = GUIPositionController.Instance.MoveJoystickPercentLocation.x * (float)width;
		_moveJoystick.PadY = GUIPositionController.Instance.MoveJoystickPercentLocation.y * (float)height;
		_moveJoystick.NubRadius = nubRadius;
	}

	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		if (_moveJoystick.IsHeld)
		{
			Vector2 inputVector = _moveJoystick.InputVector;
			if (inputVector.magnitude < DEADZONE_RADIUS)
			{
				base.Movement = Vector2.zero;
			}
			else
			{
				base.Movement = inputVector.normalized;
			}
		}
		else
		{
			base.Movement = Vector2.zero;
		}
		if (_swipe != null && _swipe.IsHeld)
		{
			base.Aiming += _swipe.SwipeShift;
		}
		base.Focus = false;
	}
}
