using UnityEngine;

public class DualJoystickControllerDirector : MoveJoystickControllerDirector
{
	private GUIJoystick _gunJoystick;

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		_gunJoystick = new GUIJoystick();
		_gunJoystick.InputDepth = 10f;
		_gunJoystick.RenderDepth = 10f;
		_gunJoystick.BaseTexture = _moveJoystick.BaseTexture;
		_gunJoystick.NubTexture = _moveJoystick.NubTexture;
		player.PlayersGUI.AddComponent(_gunJoystick);
		PositionGUI();
	}

	public override void PositionGUI()
	{
		base.PositionGUI();
		int width = Screen.width;
		int height = Screen.height;
		float num = _moveJoystick.BaseTexture.width / 2;
		float nubRadius = _moveJoystick.NubTexture.width / 2;
		if (_gunJoystick != null)
		{
			_gunJoystick.PadRadius = num;
			_gunJoystick.PadX = (float)width - (110f + num);
			_gunJoystick.PadY = (float)height - (110f + num);
			_gunJoystick.NubRadius = nubRadius;
		}
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
		player.PlayersGUI.RemoveComponent(_gunJoystick);
	}

	public override void UpdateControls(float delta)
	{
		if (_gunJoystick.IsHeld)
		{
			base.Aiming = Vector2.Scale(_gunJoystick.InputVector, new Vector2(4f, 2f));
			base.Fire = _gunJoystick.IsHeld;
		}
		else
		{
			base.Aiming = Vector2.zero;
			base.Fire = false;
		}
		base.UpdateControls(delta);
	}
}
