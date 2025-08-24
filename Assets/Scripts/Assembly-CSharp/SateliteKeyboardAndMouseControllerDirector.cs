using UnityEngine;

public class SateliteKeyboardAndMouseControllerDirector : KeyboardAndMouseControllerDirector
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
		player.PlayersGUI.AddRenderedComponent(_button.RenderDepth, _button);
		player.PlayersGUI.AddInputComponent(_button.InputDepth, _button);
		StartCooldown(0f, 0f);
		PositionGUI();
	}

	public override void RemoveFrom(PlayerController player)
	{
		base.RemoveFrom(player);
		player.PlayersGUI.RemoveRenderedComponent(_button);
		player.PlayersGUI.RemoveInputComponent(_button);
	}

	public override void PositionGUI()
	{
		base.PositionGUI();
		PositionRoundButton(_button, GUIPositionController.Instance.SatellitePercentLocation, Preferences.Instance.ButtonSize);
	}

	public override void UpdateControls(float delta)
	{
		base.UpdateControls(delta);
		base.FireSecondary = Input.GetKey(KeyCode.Tab);
	}

	public void StartCooldown(float nextUseTime, float lastUseTime)
	{
		WeaponManager weaponManager = (WeaponManager)HUD.Instance.PlayerController.WeaponManager;
		_button.StartCooldown(nextUseTime, lastUseTime);
	}
}
