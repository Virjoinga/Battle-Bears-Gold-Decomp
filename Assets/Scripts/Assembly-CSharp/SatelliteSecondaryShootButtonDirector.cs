using UnityEngine;

public class SatelliteSecondaryShootButtonDirector : ShootButtonControllerDirector
{
	private GUICooldownButton _button;

	public GUICooldownButton SecondaryButton
	{
		get
		{
			return _button;
		}
	}

	public override void AddTo(PlayerController player)
	{
		_button = new GUICooldownButton();
		_button.BackgroundTexture = Resources.Load(SimpleControllerDirector.BUTTON_BACKGROUND_TEXTURE_NAME) as Texture2D;
		if (player != null && player.CharacterManager != null && player.CharacterManager.playerLoadout != null)
		{
			string path = "Textures/GUI/" + player.CharacterManager.playerLoadout.secondary.name;
			_button.Icon = Resources.Load(path) as Texture2D;
		}
		base.AddTo(player);
		_button.HalfButtonTexture = base.SpecialButton.HalfButtonTexture;
		_button.HalfCooldownTexture = base.SpecialButton.HalfCooldownTexture;
		_button.FullCooldownTexture = base.SpecialButton.FullCooldownTexture;
		_button.InputDepth = 10f;
		player.PlayersGUI.AddRenderedComponent(_button.RenderDepth, _button);
		player.PlayersGUI.AddInputComponent(_button.InputDepth, _button);
		StartCooldown(0f, 0f);
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
		player.PlayersGUI.RemoveInputComponent(_button);
		player.PlayersGUI.RemoveRenderedComponent(_button);
	}

	public override void PositionGUI()
	{
		base.PositionGUI();
		PositionRoundButton(_button, GUIPositionController.Instance.SatellitePercentLocation, Preferences.Instance.ButtonSize);
	}

	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		if (_button.IsHeld)
		{
			base.FireSecondary = true;
		}
		else
		{
			base.FireSecondary = false;
		}
	}

	public void StartCooldown(float nextUseTime, float lastUseTime)
	{
		WeaponManager weaponManager = (WeaponManager)HUD.Instance.PlayerController.WeaponManager;
		_button.StartCooldown(nextUseTime, lastUseTime);
	}
}
