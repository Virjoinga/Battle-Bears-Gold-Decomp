using UnityEngine;

public class SimpleControllerDirector : PlayerControllerDirector
{
	private static readonly string JUMP_BUTTON_TEXTURE_NAME = "Textures/GUI/jumpButton";

	private static readonly string BLUE_SCORE_BACKGROUND_TEXTURE_NAME = "Textures/GUI/blueScoreBackground";

	private static readonly string RED_SCORE_BACKGROUND_TEXTURE_NAME = "Textures/GUI/redScoreBackground";

	private static readonly string SCORE_BACKDROP_TEXTURE_NAME = "Textures/GUI/scoreBackDrop";

	private static readonly string SETTINGS_BUTTON_TEXTURE_NAME = "Textures/GUI/settings";

	private static readonly string TEAMSPEAK_BUTTON_TEXTURE_NAME = "Textures/GUI/teamspeak";

	private static readonly string MELEE_BUTTON_TEXTURE_NAME = "Textures/GUI/melee";

	private static readonly string CLASS_PICTURE_BACKGROUND_TEXTURE_NAME = "Textures/GUI/classPictureBackground";

	protected static readonly string BUTTON_BACKGROUND_TEXTURE_NAME = "Textures/GUI/buttonBackground";

	private static readonly string BUTTON_HALF_BACKGROUND_TEXTURE_NAME = "Textures/GUI/halfButtonNoAlpha";

	private static readonly string COOLDOWN_SPINNER_HALF_TEXTURE_NAME = "Textures/GUI/cooldownSpinner";

	private static readonly string COOLDOWN_SPINNER_TEXTURE_NAME = "Textures/GUI/cooldownSpinnerFull";

	private static readonly string AMMO_ICON_TEXTURE_NAME = "Textures/GUI/ammo";

	private static readonly string AMMO_BACKDROP_TEXTURE_NAME = "Textures/GUI/ammoBackdrop";

	private static readonly string SWAP_UP_TEXTURE_NAME = "Textures/GUI/arrowUp";

	private static readonly string SWAP_DOWN_TEXTURE_NAME = "Textures/GUI/arrowDown";

	private static readonly string HEALTH_BAR_TEXTURE_NAME = "Textures/GUI/healthBar";

	private static readonly string HEALTH_BAR_OUTLINE_TEXTURE_NAME = "Textures/GUI/healthBarOutline5Px";

	private float _optionsButtonScale = 0.5f;

	private float _scoreAreaScale = 0.9f;

	private float _scoreBackgroundScale = 1.15f;

	private float _ammoScale = 0.75f;

	private int _smallFontSize = 38;

	private int _largeFontSize = 42;

	private GUIRoundButton _jumpButton;

	private GUIRoundButton _meleeButton;

	private GUICooldownButton _specialButton;

	private GUIRoundButton _teamspeakButton;

	private GUITextArea _redScore;

	private GUITextArea _blueScore;

	private GUITextArea _ammoText;

	private GUITextArea _ammoSlash;

	private GUITextArea _clipText;

	private GUIRectangleButton _ammoBackdrop;

	private GUIRectangleButton _swapButton;

	private SlideableGUIElement _healthBar;

	private GUITextArea _healthText;

	private GUIRoundButton _optionsButton;

	private DecorativeGUIElement _redScoreBackground;

	private DecorativeGUIElement _blueScoreBackgound;

	private DecorativeGUIElement _scoreBackdrop;

	private DecorativeGUIElement _ammoIcon;

	private DecorativeGUIElement _swapUpArrow;

	private DecorativeGUIElement _swapDownArrow;

	private DecorativeGUIElement _healthBarBackdrop;

	private DecorativeGUIElement _healthBarOutline;

	public GUIRoundButton JumpButton
	{
		get
		{
			return _jumpButton;
		}
	}

	public GUIRoundButton MeleeButton
	{
		get
		{
			return _meleeButton;
		}
	}

	public GUICooldownButton SpecialButton
	{
		get
		{
			return _specialButton;
		}
	}

	public GUIRoundButton TeamspeakButton
	{
		get
		{
			return _teamspeakButton;
		}
	}

	public GUITextArea RedScore
	{
		get
		{
			return _redScore;
		}
	}

	public GUITextArea BlueScore
	{
		get
		{
			return _blueScore;
		}
	}

	public GUITextArea AmmoText
	{
		get
		{
			return _ammoText;
		}
	}

	public GUITextArea ClipText
	{
		get
		{
			return _clipText;
		}
	}

	public GUIRectangleButton ReloadButton
	{
		get
		{
			return _ammoBackdrop;
		}
	}

	public GUIRectangleButton SwitchButton
	{
		get
		{
			return _swapButton;
		}
	}

	public SlideableGUIElement HealthBar
	{
		get
		{
			return _healthBar;
		}
	}

	public GUITextArea HealthText
	{
		get
		{
			return _healthText;
		}
	}

	public override void AddTo(PlayerController player)
	{
		InitButtons();
		if (Preferences.Instance.IsTeamMode)
		{
			InitScoreAreaTeam();
			player.PlayersGUI.AddComponent(_redScoreBackground);
			player.PlayersGUI.AddComponent(_blueScoreBackgound);
			player.PlayersGUI.AddComponent(_scoreBackdrop);
			player.PlayersGUI.AddComponent(_redScore);
			player.PlayersGUI.AddComponent(_blueScore);
		}
		InitOptionsButton();
		player.PlayersGUI.AddComponent(_optionsButton);
		bool flag = false;
		if (LoadoutManager.Instance != null && LoadoutManager.Instance.CurrentLoadout != null)
		{
			flag = LoadoutManager.Instance.CurrentLoadout.model.name == "Huggable";
		}
		if (!flag || Tutorial.Instance != null)
		{
			InitAmmoArea();
			player.PlayersGUI.AddComponent(_ammoBackdrop);
			player.PlayersGUI.AddComponent(_ammoIcon);
			player.PlayersGUI.AddComponent(_ammoText);
			player.PlayersGUI.AddComponent(_ammoSlash);
			player.PlayersGUI.AddComponent(_clipText);
		}
		InitSwapArea();
		InitHealthBarArea(player);
		if (HUD.Instance.JumpButtonEnabled)
		{
			player.PlayersGUI.AddComponent(_jumpButton);
		}
		player.PlayersGUI.AddComponent(_swapButton);
		player.PlayersGUI.AddComponent(_meleeButton);
		player.PlayersGUI.AddComponent(_specialButton);
		player.PlayersGUI.AddComponent(_teamspeakButton);
		player.PlayersGUI.AddComponent(_swapUpArrow);
		player.PlayersGUI.AddComponent(_swapDownArrow);
		player.PlayersGUI.AddComponent(_healthBar);
		player.PlayersGUI.AddComponent(_healthBarBackdrop);
		player.PlayersGUI.AddComponent(_healthBarOutline);
		player.PlayersGUI.AddComponent(_healthText);
	}

	private void InitButtons()
	{
		Texture2D backgroundTexture = Resources.Load(BUTTON_BACKGROUND_TEXTURE_NAME) as Texture2D;
		if (HUD.Instance.JumpButtonEnabled)
		{
			_jumpButton = new GUIRoundButton();
			_jumpButton.BackgroundTexture = backgroundTexture;
			_jumpButton.Icon = Resources.Load(JUMP_BUTTON_TEXTURE_NAME) as Texture2D;
			_jumpButton.InputDepth = 10f;
		}
		_meleeButton = new GUIRoundButton();
		_meleeButton.BackgroundTexture = backgroundTexture;
		_meleeButton.Icon = Resources.Load(MELEE_BUTTON_TEXTURE_NAME) as Texture2D;
		_meleeButton.InputDepth = 10f;
		_specialButton = new GUICooldownButton();
		_specialButton.BackgroundTexture = backgroundTexture;
		_specialButton.HalfButtonTexture = Resources.Load(BUTTON_HALF_BACKGROUND_TEXTURE_NAME) as Texture2D;
		_specialButton.HalfCooldownTexture = Resources.Load(COOLDOWN_SPINNER_HALF_TEXTURE_NAME) as Texture2D;
		_specialButton.FullCooldownTexture = Resources.Load(COOLDOWN_SPINNER_TEXTURE_NAME) as Texture2D;
		_specialButton.InputDepth = 10f;
		_specialButton.StartCooldown(HUD.Instance.PlayerController.NextSpecialItemChargeTime, HUD.Instance.PlayerController.LastSpecialItemUseTime);
		_teamspeakButton = new GUIRoundButton();
		_teamspeakButton.BackgroundTexture = Resources.Load(TEAMSPEAK_BUTTON_TEXTURE_NAME) as Texture2D;
		_teamspeakButton.InputDepth = 10f;
	}

	private void InitScoreAreaTeam()
	{
		_redScoreBackground = new DecorativeGUIElement();
		_redScoreBackground.Icon = Resources.Load(RED_SCORE_BACKGROUND_TEXTURE_NAME) as Texture2D;
		_redScoreBackground.CenterOnPosXY = true;
		_redScoreBackground.UseWhiteTint = true;
		_redScoreBackground.RenderDepth = 10f;
		_blueScoreBackgound = new DecorativeGUIElement();
		_blueScoreBackgound.Icon = Resources.Load(BLUE_SCORE_BACKGROUND_TEXTURE_NAME) as Texture2D;
		_blueScoreBackgound.CenterOnPosXY = true;
		_blueScoreBackgound.UseWhiteTint = true;
		_blueScoreBackgound.RenderDepth = 10f;
		_scoreBackdrop = new DecorativeGUIElement();
		_scoreBackdrop.Icon = Resources.Load(SCORE_BACKDROP_TEXTURE_NAME) as Texture2D;
		_scoreBackdrop.CenterOnPosXY = true;
		_scoreBackdrop.UseWhiteTint = false;
		_redScore = new GUITextArea();
		_redScore.CenterOnPosXY = true;
		_redScore.UseWhiteTint = true;
		_redScore.Text = HUD.Instance.redKills;
		_redScore.DefaultHeight = _blueScoreBackgound.Icon.height;
		_redScore.DefaultWidth = _blueScoreBackgound.Icon.width;
		_redScore.RenderDepth = 20f;
		_blueScore = new GUITextArea();
		_blueScore.CenterOnPosXY = true;
		_blueScore.UseWhiteTint = true;
		_blueScore.Text = HUD.Instance.blueKills;
		_blueScore.DefaultHeight = _redScoreBackground.Icon.height;
		_blueScore.DefaultWidth = _redScoreBackground.Icon.width;
		_blueScore.RenderDepth = 20f;
	}

	private void InitOptionsButton()
	{
		_optionsButton = new GUIRoundButton();
		_optionsButton.BackgroundTexture = Resources.Load(SETTINGS_BUTTON_TEXTURE_NAME) as Texture2D;
		_optionsButton.InputDepth = 10f;
		_optionsButton.RenderDepth = 11f;
	}

	private void InitAmmoArea()
	{
		_ammoBackdrop = new GUIRectangleButton();
		_ammoBackdrop.BackgroundTexture = Resources.Load(AMMO_BACKDROP_TEXTURE_NAME) as Texture2D;
		_ammoBackdrop.InputDepth = 10f;
		_ammoIcon = new DecorativeGUIElement();
		_ammoIcon.Icon = Resources.Load(AMMO_ICON_TEXTURE_NAME) as Texture2D;
		_ammoIcon.CenterOnPosXY = true;
		_ammoIcon.UseWhiteTint = true;
		_ammoIcon.RenderDepth = 10f;
		_ammoText = new GUITextArea();
		_ammoText.CenterOnPosXY = true;
		_ammoText.UseWhiteTint = true;
		_ammoText.DefaultHeight = _ammoBackdrop.BackgroundTexture.height;
		_ammoText.DefaultWidth = _ammoBackdrop.BackgroundTexture.width;
		_ammoText.Text = HUD.Instance.CurrentAmmo;
		_ammoText.RenderDepth = 20f;
		_ammoSlash = new GUITextArea();
		_ammoSlash.CenterOnPosXY = true;
		_ammoSlash.UseWhiteTint = true;
		_ammoSlash.DefaultHeight = _ammoBackdrop.BackgroundTexture.height;
		_ammoSlash.DefaultWidth = _ammoBackdrop.BackgroundTexture.width;
		_ammoSlash.Text = "/";
		_ammoSlash.FontSize = 22;
		_ammoSlash.RenderDepth = 20f;
		_clipText = new GUITextArea();
		_clipText.CenterOnPosXY = true;
		_clipText.UseWhiteTint = true;
		_clipText.DefaultHeight = _ammoBackdrop.BackgroundTexture.height;
		_clipText.DefaultWidth = _ammoBackdrop.BackgroundTexture.width;
		_clipText.Text = HUD.Instance.ClipSize;
		_clipText.FontSize = 22;
		_clipText.RenderDepth = 20f;
	}

	private void InitSwapArea()
	{
		_swapUpArrow = new DecorativeGUIElement();
		_swapUpArrow.Icon = Resources.Load(SWAP_UP_TEXTURE_NAME) as Texture2D;
		_swapUpArrow.CenterOnPosXY = true;
		_swapDownArrow = new DecorativeGUIElement();
		_swapDownArrow.Icon = Resources.Load(SWAP_DOWN_TEXTURE_NAME) as Texture2D;
		_swapDownArrow.CenterOnPosXY = true;
		_swapButton = new GUIRectangleButton();
		_swapButton.InputDepth = 10f;
		_swapButton.RenderDepth = 11f;
	}

	private void InitHealthBarArea(PlayerController player)
	{
		_healthBar = new SlideableGUIElement();
		_healthBar.Icon = Resources.Load(HEALTH_BAR_TEXTURE_NAME) as Texture2D;
		_healthBar.RenderDepth = 15f;
		_healthBar.DesiredSlidePercent = HUD.Instance.HealthPercentage;
		_healthBarBackdrop = new DecorativeGUIElement();
		_healthBarBackdrop.Icon = _healthBar.Icon;
		_healthBarBackdrop.RenderDepth = 12f;
		_healthBarBackdrop.UseSecondaryGUIColor = true;
		_healthBarOutline = new DecorativeGUIElement();
		_healthBarOutline.Icon = Resources.Load(HEALTH_BAR_OUTLINE_TEXTURE_NAME) as Texture2D;
		_healthBarOutline.RenderDepth = 10f;
		_healthText = new GUITextArea();
		_healthText.CenterOnPosXY = false;
		_healthText.UseBlackTint = true;
		_healthText.DefaultHeight = _healthBarOutline.Icon.height;
		_healthText.DefaultWidth = _healthBarOutline.Icon.width;
		_healthText.Text = GetHealthText();
		_healthText.FontSize = 22;
		_healthText.RenderDepth = 16f;
	}

	public override void RemoveFrom(PlayerController player)
	{
		if (_jumpButton != null)
		{
			player.PlayersGUI.RemoveComponent(_jumpButton);
		}
		player.PlayersGUI.RemoveComponent(_meleeButton);
		player.PlayersGUI.RemoveComponent(_specialButton);
		player.PlayersGUI.RemoveComponent(_teamspeakButton);
		player.PlayersGUI.RemoveComponent(_redScoreBackground);
		player.PlayersGUI.RemoveComponent(_blueScoreBackgound);
		player.PlayersGUI.RemoveComponent(_scoreBackdrop);
		player.PlayersGUI.RemoveComponent(_optionsButton);
		player.PlayersGUI.RemoveComponent(_redScore);
		player.PlayersGUI.RemoveComponent(_blueScore);
		player.PlayersGUI.RemoveComponent(_ammoBackdrop);
		player.PlayersGUI.RemoveComponent(_ammoIcon);
		player.PlayersGUI.RemoveComponent(_ammoText);
		player.PlayersGUI.RemoveComponent(_ammoSlash);
		player.PlayersGUI.RemoveComponent(_clipText);
		player.PlayersGUI.RemoveComponent(_swapButton);
		player.PlayersGUI.RemoveComponent(_swapUpArrow);
		player.PlayersGUI.RemoveComponent(_swapDownArrow);
		player.PlayersGUI.RemoveComponent(_healthBar);
		player.PlayersGUI.RemoveComponent(_healthBarBackdrop);
		player.PlayersGUI.RemoveComponent(_healthBarOutline);
		player.PlayersGUI.RemoveComponent(_healthText);
	}

	public virtual void PositionGUI()
	{
		if (_jumpButton != null)
		{
			PositionRoundButton(_jumpButton, GUIPositionController.Instance.JumpButtonPercentLocation, Preferences.Instance.ButtonSize);
		}
		PositionRoundButton(_meleeButton, GUIPositionController.Instance.MeleeButtonPercentLocation, Preferences.Instance.ButtonSize);
		PositionRoundButton(_specialButton, GUIPositionController.Instance.SpecialButtonPercentLocation, Preferences.Instance.ButtonSize);
		PositionRoundButton(_teamspeakButton, GUIPositionController.Instance.TeamspeakButtonPercentLocation, Preferences.Instance.ButtonSize);
		if (_optionsButton != null)
		{
			PositionRoundButton(_optionsButton, GUIPositionController.Instance.ScoreBackdropPercentLocation + (Preferences.Instance.IsTeamMode ? GUIPositionController.Instance.OptionsButtonPercentLocation : GUIPositionController.Instance.OptionsButtonFFAPercentLocation), _optionsButtonScale);
		}
		if (Preferences.Instance.IsTeamMode)
		{
			PositionDecorativeElement(_redScoreBackground, GUIPositionController.Instance.ScoreBackdropPercentLocation + GUIPositionController.Instance.RedScoreBackgroundPercentLocation, _scoreAreaScale);
			PositionDecorativeElement(_blueScoreBackgound, GUIPositionController.Instance.ScoreBackdropPercentLocation + GUIPositionController.Instance.BlueScoreBackgroundPercentLocation, _scoreAreaScale);
			PositionDecorativeElement(_scoreBackdrop, GUIPositionController.Instance.ScoreBackdropPercentLocation, _scoreBackgroundScale);
			PositionTextElement(_redScore, GUIPositionController.Instance.ScoreBackdropPercentLocation + GUIPositionController.Instance.RedScoreTextPercentLocation);
			PositionTextElement(_blueScore, GUIPositionController.Instance.ScoreBackdropPercentLocation + GUIPositionController.Instance.BlueScoreTextPercentLocation);
		}
		if (_ammoBackdrop != null)
		{
			PositionSquareButton(_ammoBackdrop, GUIPositionController.Instance.AmmoBackdropPercentLocation, _ammoScale);
			PositionDecorativeElement(_ammoIcon, GUIPositionController.Instance.AmmoBackdropPercentLocation + GUIPositionController.Instance.AmmoIconPercentLocation);
			PositionTextElement(_ammoText, GUIPositionController.Instance.AmmoBackdropPercentLocation + GUIPositionController.Instance.AmmoTextPercentLocation);
			PositionTextElement(_ammoSlash, GUIPositionController.Instance.AmmoBackdropPercentLocation + GUIPositionController.Instance.AmmoSlashPercentLocation);
			PositionTextElement(_clipText, GUIPositionController.Instance.AmmoBackdropPercentLocation + GUIPositionController.Instance.ClipTextPercentLocation);
		}
		PositionDecorativeElement(_swapUpArrow, GUIPositionController.Instance.SwapWeaponAreaPercentLocation + GUIPositionController.Instance.SwapUpArrowPercentLocation);
		PositionDecorativeElement(_swapDownArrow, GUIPositionController.Instance.SwapWeaponAreaPercentLocation + GUIPositionController.Instance.SwapDownArrowPercentLocation);
		float x = (1f - GUIPositionController.Instance.SwapWeaponAreaPercentLocation.x) * (float)Screen.width * 2f;
		float y = (GUIPositionController.Instance.SwapWeaponAreaPercentLocation.y + GUIPositionController.Instance.SwapDownArrowPercentLocation.y) * (float)Screen.height * 2f;
		PositionSquareButton(_swapButton, GUIPositionController.Instance.SwapWeaponAreaPercentLocation, new Vector2(x, y));
		PositionDecorativeElement(_healthBar, GUIPositionController.Instance.HealthBarAreaPercentLocation);
		PositionDecorativeElement(_healthBarBackdrop, GUIPositionController.Instance.HealthBarAreaPercentLocation);
		PositionDecorativeElement(_healthBarOutline, GUIPositionController.Instance.HealthBarAreaPercentLocation);
		PositionTextElement(_healthText, GUIPositionController.Instance.HealthBarAreaPercentLocation + GUIPositionController.Instance.HealthTextPercentLocation);
	}

	protected void PositionRoundButton(GUIRoundButton button, Vector2 percentScreenLocation, float scale = 1f)
	{
		button.ButtonX = (float)Screen.width * percentScreenLocation.x;
		button.ButtonY = (float)Screen.height * percentScreenLocation.y;
		float num = ((!(button.BackgroundTexture == null)) ? button.BackgroundTexture.width : button.Icon.width);
		button.ButtonRadius = num * scale * PlayerGUI.Instance.SmallestRatio;
	}

	protected void PositionSquareButton(GUIRectangleButton button, Vector2 percentScreenLocation, float scale = 1f)
	{
		button.ButtonX = (float)Screen.width * percentScreenLocation.x;
		button.ButtonY = (float)Screen.height * percentScreenLocation.y;
		float num = -1f;
		if (button.BackgroundTexture != null)
		{
			num = button.BackgroundTexture.width;
		}
		else if (button.Icon != null)
		{
			num = button.Icon.width;
		}
		float num2 = -1f;
		if (button.BackgroundTexture != null)
		{
			num2 = button.BackgroundTexture.height;
		}
		else if (button.Icon != null)
		{
			num2 = button.Icon.height;
		}
		button.ButtonWidth = num * scale * PlayerGUI.Instance.SmallestRatio;
		button.ButtonHeight = num2 * scale * PlayerGUI.Instance.SmallestRatio;
	}

	protected void PositionSquareButton(GUIRectangleButton button, Vector2 percentScreenLocation, Vector2 sizeOverride, float scale = 1f)
	{
		button.ButtonX = (float)Screen.width * percentScreenLocation.x;
		button.ButtonY = (float)Screen.height * percentScreenLocation.y;
		button.ButtonWidth = sizeOverride.x * scale;
		button.ButtonHeight = sizeOverride.y * scale;
	}

	protected void PositionDecorativeElement(DecorativeGUIElement element, Vector2 percentScreenLocation, float scale = 1f)
	{
		element.PositionX = (float)Screen.width * percentScreenLocation.x;
		element.PositionY = (float)Screen.height * percentScreenLocation.y;
		element.Width = (float)element.Icon.width * scale * PlayerGUI.Instance.SmallestRatio;
		element.Height = (float)element.Icon.height * scale * PlayerGUI.Instance.SmallestRatio;
	}

	protected void PositionTextElement(GUITextArea textArea, Vector2 percentScreenLocation, float scale = 1f)
	{
		textArea.PositionX = (float)Screen.width * percentScreenLocation.x;
		textArea.PositionY = (float)Screen.height * percentScreenLocation.y;
		textArea.Width = textArea.DefaultWidth * scale * PlayerGUI.Instance.SmallestRatio;
		textArea.Height = textArea.DefaultHeight * scale * PlayerGUI.Instance.SmallestRatio;
	}

	public override void UpdateControls(float delta)
	{
		if (_jumpButton != null && _jumpButton.IsHeld)
		{
			base.Jump = true;
		}
		else
		{
			base.Jump = false;
		}
		if (_meleeButton.IsHeld)
		{
			base.Melee = true;
		}
		else
		{
			base.Melee = false;
		}
		if (_specialButton.IsHeld)
		{
			base.Special = true;
		}
		else
		{
			base.Special = false;
		}
		if (_optionsButton != null && _optionsButton.IsHeld)
		{
			base.Pause = true;
		}
		else
		{
			base.Pause = false;
		}
		if (_ammoBackdrop != null && _ammoBackdrop.IsHeld)
		{
			base.Reload = true;
		}
		else
		{
			base.Reload = false;
		}
		if (_teamspeakButton.IsHeld)
		{
			base.Teamspeak = true;
		}
		else
		{
			base.Teamspeak = false;
		}
		if (_swapButton != null && _swapButton.WasPressed)
		{
			base.Switch = true;
		}
		else
		{
			base.Switch = false;
		}
	}

	public override void UpdateTextValues()
	{
		SetTextValues();
	}

	public void ToggleSwapArea(bool state)
	{
		if (_swapUpArrow != null && _swapDownArrow != null)
		{
			_swapUpArrow.Enabled = state;
			_swapDownArrow.Enabled = state;
			_swapButton.Enabled = state;
		}
	}

	public void ToggleAmmoArea(bool state)
	{
		if (_ammoBackdrop != null && _ammoIcon != null && _ammoText != null && _ammoSlash != null && _clipText != null)
		{
			_ammoBackdrop.Enabled = state;
			_ammoIcon.Enabled = state;
			_ammoText.Enabled = state;
			_ammoSlash.Enabled = state;
			_clipText.Enabled = state;
		}
	}

	private void SetTextValues()
	{
		if (Preferences.Instance.IsTeamMode)
		{
			_redScore.Text = HUD.Instance.redKills;
			_blueScore.Text = HUD.Instance.blueKills;
		}
		if (_ammoText != null && _clipText != null)
		{
			_ammoText.Text = HUD.Instance.CurrentAmmo;
			_clipText.Text = HUD.Instance.ClipSize;
			if (_clipText.Text != null && _clipText.Text.Length > 2)
			{
				_ammoText.FontSize = _smallFontSize;
			}
			else
			{
				_ammoText.FontSize = _largeFontSize;
			}
		}
		_healthText.Text = GetHealthText();
	}

	private string GetHealthText()
	{
		string result = "0/100";
		if (HUD.Instance.PlayerController != null && HUD.Instance.PlayerController.DamageReceiver != null)
		{
			float num = ((!HUD.Instance.PlayerController.IsDead) ? HUD.Instance.PlayerController.DamageReceiver.CurrentHP : 0f);
			result = (int)num + "/" + (int)HUD.Instance.PlayerController.DamageReceiver.startHealth;
		}
		return result;
	}
}
