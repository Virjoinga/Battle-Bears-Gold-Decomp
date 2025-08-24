using UnityEngine;

public class ShootButtonControllerDirector : MoveJoystickControllerDirector
{
	private float _gunButtonAdditonalScale = 0.6f;

	private GUIGunButtonSwipe _gunButton;

	public GUIGunButtonSwipe GunButton
	{
		get
		{
			return _gunButton;
		}
	}

	public override void AddTo(PlayerController player)
	{
		base.AddTo(player);
		_gunButton = new GUIGunButtonSwipe();
		_gunButton.InputDepth = 10f;
		_gunButton.RenderDepth = 10f;
		_gunButton.NubTexture = _moveJoystick.BaseTexture;
		player.PlayersGUI.AddComponent(_gunButton);
		PositionGUI();
	}

	public override void PositionGUI()
	{
		base.PositionGUI();
		if (_gunButton != null)
		{
			int width = Screen.width;
			int height = Screen.height;
			float num = (float)_gunButton.NubTexture.width * Preferences.Instance.ButtonSize * PlayerGUI.Instance.SmallestRatio;
			_gunButton.PadRadius = num * _gunButtonAdditonalScale;
			_gunButton.PadX = GUIPositionController.Instance.GunButtonPercentLocation.x * (float)width;
			_gunButton.PadY = GUIPositionController.Instance.GunButtonPercentLocation.y * (float)height;
			_gunButton.NubRadius = num * _gunButtonAdditonalScale;
		}
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
		player.PlayersGUI.RemoveComponent(_gunButton);
	}

	public override void UpdateControls(float delta)
	{
		if (_gunButton.IsHeld)
		{
			base.Aiming = _gunButton.SwipeDelta;
			base.Fire = _gunButton.IsHeld;
		}
		else
		{
			base.Aiming = Vector2.zero;
			base.Fire = false;
		}
		base.UpdateControls(delta);
	}
}
