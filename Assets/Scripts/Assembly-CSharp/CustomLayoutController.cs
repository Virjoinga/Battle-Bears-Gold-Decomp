using UnityEngine;

public class CustomLayoutController
{
	private static string BACKPLATE_ACCEPT_BUTTON_TEXTURE_NAME = "Textures/GUI/backPlateLongAccept";

	private static string BACKPLATE_RESET_BUTTON_TEXTURE_NAME = "Textures/GUI/backPlateLongReset";

	private static string MOVE_AREA_TEXTURE_NAME = "Textures/GUI/moveArea";

	private static string CHECK_TEXTURE_NAME = "Textures/GUI/check";

	private static string RELOAD_TEXTURE_NAME = "Textures/GUI/reload";

	private static string CUSTOM_LAYOUT_PREFAB_PATH = "CustomControlsPopup";

	private static CustomLayoutController _instance;

	private GameObject _blackBgObject;

	private bool _shouldClose;

	private float _buttonOffset = 0.05f;

	private MoveableGUIButton _moveJoystickArea;

	private MoveableGUIButton _moveRadarArea;

	private MoveableGUIButton _shootButtonArea;

	private MoveableGUIButton _satelliteButtonArea;

	private MoveableGUIButton _specialButtonArea;

	private MoveableGUIButton _jumpButtonArea;

	private MoveableGUIButton _meleeButtonArea;

	private MoveableGUIButton _reloadButtonArea;

	private MoveableGUIButton _teamspeakArea;

	private ResetPositionGUIButton _resetPositionButton;

	private AcceptChangesGUIButton _acceptButton;

	public static CustomLayoutController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CustomLayoutController();
			}
			return _instance;
		}
	}

	public bool IsOpen { get; private set; }

	public void ResetMoveableAreas()
	{
		if (_moveJoystickArea != null)
		{
			ResetButtonArea(_moveJoystickArea, GUIPositionController.Instance.MoveJoystickPercentLocation);
		}
		if (_moveRadarArea != null)
		{
			_moveRadarArea.ButtonX = GUIPositionController.Instance.RadarPercentLocation.x * (float)Screen.width + _moveRadarArea.ButtonWidth / 2f;
			_moveRadarArea.ButtonY = GUIPositionController.Instance.RadarPercentLocation.y * (float)Screen.height + _moveRadarArea.ButtonHeight / 2f;
		}
		if (_meleeButtonArea != null)
		{
			ResetButtonArea(_meleeButtonArea, GUIPositionController.Instance.MeleeButtonPercentLocation);
		}
		if (_jumpButtonArea != null)
		{
			ResetButtonArea(_jumpButtonArea, GUIPositionController.Instance.JumpButtonPercentLocation);
		}
		if (_specialButtonArea != null)
		{
			ResetButtonArea(_specialButtonArea, GUIPositionController.Instance.SpecialButtonPercentLocation);
		}
		if (_reloadButtonArea != null)
		{
			ResetButtonArea(_reloadButtonArea, GUIPositionController.Instance.AmmoBackdropPercentLocation);
		}
		if (_shootButtonArea != null)
		{
			ResetButtonArea(_shootButtonArea, GUIPositionController.Instance.GunButtonPercentLocation);
		}
		if (_teamspeakArea != null)
		{
			ResetButtonArea(_teamspeakArea, GUIPositionController.Instance.TeamspeakButtonPercentLocation);
		}
		if (_satelliteButtonArea != null)
		{
			ResetButtonArea(_satelliteButtonArea, GUIPositionController.Instance.SatellitePercentLocation);
		}
	}

	private void ResetButtonArea(MoveableGUIButton button, Vector2 location)
	{
		button.ButtonX = location.x * (float)Screen.width;
		button.ButtonY = location.y * (float)Screen.height;
	}

	public void Show()
	{
		if (_blackBgObject == null)
		{
			_blackBgObject = Object.Instantiate(Resources.Load(CUSTOM_LAYOUT_PREFAB_PATH)) as GameObject;
			_blackBgObject.transform.parent = HUD.Instance.transform;
			_blackBgObject.transform.localPosition = new Vector3(0f, 9f, -275f);
			_blackBgObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		_blackBgObject.SetActive(true);
		IsOpen = true;
		if (HUD.Instance.PlayerController != null)
		{
			SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)HUD.Instance.PlayerController.Performer;
			if (simpleControllerPerformer != null)
			{
				simpleControllerPerformer.Disabled = true;
			}
			GUIPositionController.Instance.LoadFromPlayerPrefs();
			CreateMoveableControls();
		}
	}

	private void CreateMoveableControls()
	{
		float num = Preferences.Instance.ButtonSize * 2f;
		Texture2D texture2D = Resources.Load(MOVE_AREA_TEXTURE_NAME) as Texture2D;
		ShootButtonControllerDirector shootButtonControllerDirector = null;
		if (HUD.Instance.PlayerController.Director is ShootButtonControllerDirector)
		{
			shootButtonControllerDirector = (ShootButtonControllerDirector)HUD.Instance.PlayerController.Director;
		}
		MoveJoystickControllerDirector moveJoystickControllerDirector = null;
		if (HUD.Instance.PlayerController.Director is MoveJoystickControllerDirector)
		{
			moveJoystickControllerDirector = (MoveJoystickControllerDirector)HUD.Instance.PlayerController.Director;
		}
		SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)HUD.Instance.PlayerController.Director;
		if (moveJoystickControllerDirector != null)
		{
			InitMoveArea(ref _moveJoystickArea, GUIPositionController.Instance.MoveJoystickPercentLocation, (float)moveJoystickControllerDirector.MoveJoystick.BaseTexture.width * num, (float)moveJoystickControllerDirector.MoveJoystick.BaseTexture.height * num, texture2D);
		}
		if (HUD.Instance.RadarPurchased && HUD.Instance.RadarElement != null && Preferences.Instance.RadarToggledOn)
		{
			_moveRadarArea = new MoveableGUIButton();
			_moveRadarArea.ButtonWidth = HUD.Instance.RadarElement.RadarBaseRect.width;
			_moveRadarArea.ButtonHeight = HUD.Instance.RadarElement.RadarBaseRect.height;
			_moveRadarArea.ButtonX = GUIPositionController.Instance.RadarPercentLocation.x * (float)Screen.width + _moveRadarArea.ButtonWidth / 2f;
			_moveRadarArea.ButtonY = GUIPositionController.Instance.RadarPercentLocation.y * (float)Screen.height + _moveRadarArea.ButtonHeight / 2f;
			_moveRadarArea.BackgroundTexture = texture2D;
			_moveRadarArea.RenderDepth = 30f;
			_moveRadarArea.InputDepth = 30f;
			PlayerGUI.Instance.AddComponent(_moveRadarArea);
		}
		if (simpleControllerDirector != null)
		{
			InitMoveArea(ref _meleeButtonArea, GUIPositionController.Instance.MeleeButtonPercentLocation, (float)simpleControllerDirector.MeleeButton.BackgroundTexture.width * num, (float)simpleControllerDirector.MeleeButton.BackgroundTexture.height * num, texture2D);
			if (HUD.Instance.JumpButtonEnabled)
			{
				InitMoveArea(ref _jumpButtonArea, GUIPositionController.Instance.JumpButtonPercentLocation, (float)simpleControllerDirector.JumpButton.BackgroundTexture.width * num, (float)simpleControllerDirector.JumpButton.BackgroundTexture.height * num, texture2D);
			}
			if (HUD.Instance.PlayerController.hasSpecialItem())
			{
				InitMoveArea(ref _specialButtonArea, GUIPositionController.Instance.SpecialButtonPercentLocation, (float)simpleControllerDirector.SpecialButton.BackgroundTexture.width * num, (float)simpleControllerDirector.SpecialButton.BackgroundTexture.height * num, texture2D);
			}
			if (simpleControllerDirector.ReloadButton != null)
			{
				InitMoveArea(ref _reloadButtonArea, GUIPositionController.Instance.AmmoBackdropPercentLocation, simpleControllerDirector.ReloadButton.BackgroundTexture.width, simpleControllerDirector.ReloadButton.BackgroundTexture.height, texture2D);
			}
			InitMoveArea(ref _teamspeakArea, GUIPositionController.Instance.TeamspeakButtonPercentLocation, (float)simpleControllerDirector.TeamspeakButton.BackgroundTexture.width * num, (float)simpleControllerDirector.TeamspeakButton.BackgroundTexture.height * num, texture2D);
		}
		if (shootButtonControllerDirector != null)
		{
			InitMoveArea(ref _shootButtonArea, GUIPositionController.Instance.GunButtonPercentLocation, (float)shootButtonControllerDirector.GunButton.NubTexture.width * num, (float)shootButtonControllerDirector.GunButton.NubTexture.height * num, texture2D);
		}
		if (HUD.Instance.PlayerController.SatelliteSecondaries)
		{
			SetupSatelliteMoveArea(texture2D, num);
		}
		_resetPositionButton = new ResetPositionGUIButton();
		_resetPositionButton.ButtonX = Screen.width / 2;
		_resetPositionButton.ButtonY = (float)(Screen.height / 2) + (float)Screen.height * _buttonOffset;
		_resetPositionButton.BackgroundTexture = Resources.Load(BACKPLATE_RESET_BUTTON_TEXTURE_NAME) as Texture2D;
		_resetPositionButton.Icon = Resources.Load(RELOAD_TEXTURE_NAME) as Texture2D;
		_resetPositionButton.IconPercentOffset = new Vector2(0.32f, 0f);
		_resetPositionButton.IconScale = 0.6f;
		_resetPositionButton.ButtonWidth = (float)_resetPositionButton.BackgroundTexture.width * PlayerGUI.Instance.SmallestRatio;
		_resetPositionButton.ButtonHeight = (float)_resetPositionButton.BackgroundTexture.height * PlayerGUI.Instance.SmallestRatio;
		_resetPositionButton.InputDepth = 35f;
		_resetPositionButton.RenderDepth = 35f;
		_acceptButton = new AcceptChangesGUIButton();
		_acceptButton.ButtonX = Screen.width / 2;
		_acceptButton.ButtonY = (float)(Screen.height / 2) - (float)Screen.height * _buttonOffset;
		_acceptButton.BackgroundTexture = Resources.Load(BACKPLATE_ACCEPT_BUTTON_TEXTURE_NAME) as Texture2D;
		_acceptButton.Icon = Resources.Load(CHECK_TEXTURE_NAME) as Texture2D;
		_acceptButton.IconPercentOffset = new Vector2(0.32f, 0f);
		_acceptButton.IconScale = 0.6f;
		_acceptButton.ButtonWidth = (float)_resetPositionButton.BackgroundTexture.width * PlayerGUI.Instance.SmallestRatio;
		_acceptButton.ButtonHeight = (float)_resetPositionButton.BackgroundTexture.height * PlayerGUI.Instance.SmallestRatio;
		_acceptButton.InputDepth = 35f;
		_acceptButton.RenderDepth = 35f;
		PlayerGUI.Instance.AddComponent(_resetPositionButton);
		PlayerGUI.Instance.AddComponent(_acceptButton);
	}

	private void SetupSatelliteMoveArea(Texture2D moveAreaTex, float buttonScale)
	{
		if (HUD.Instance.PlayerController.Director is SatelliteSecondaryShootButtonDirector)
		{
			SatelliteSecondaryShootButtonDirector satelliteSecondaryShootButtonDirector = (SatelliteSecondaryShootButtonDirector)HUD.Instance.PlayerController.Director;
			InitMoveArea(ref _satelliteButtonArea, GUIPositionController.Instance.SatellitePercentLocation, (float)satelliteSecondaryShootButtonDirector.SecondaryButton.BackgroundTexture.width * buttonScale, (float)satelliteSecondaryShootButtonDirector.SecondaryButton.BackgroundTexture.height * buttonScale, moveAreaTex);
		}
		else if (HUD.Instance.PlayerController.Director is SatelliteSecondaryDoubleTapDirector)
		{
			SatelliteSecondaryDoubleTapDirector satelliteSecondaryDoubleTapDirector = (SatelliteSecondaryDoubleTapDirector)HUD.Instance.PlayerController.Director;
			InitMoveArea(ref _satelliteButtonArea, GUIPositionController.Instance.SatellitePercentLocation, (float)satelliteSecondaryDoubleTapDirector.SecondaryButton.BackgroundTexture.width * buttonScale, (float)satelliteSecondaryDoubleTapDirector.SecondaryButton.BackgroundTexture.height * buttonScale, moveAreaTex);
		}
		else if (HUD.Instance.PlayerController.Director is SatelliteMogaControllerDirector)
		{
			SatelliteMogaControllerDirector satelliteMogaControllerDirector = (SatelliteMogaControllerDirector)HUD.Instance.PlayerController.Director;
			InitMoveArea(ref _satelliteButtonArea, GUIPositionController.Instance.SatellitePercentLocation, (float)satelliteMogaControllerDirector.SecondaryButton.BackgroundTexture.width * buttonScale, (float)satelliteMogaControllerDirector.SecondaryButton.BackgroundTexture.height * buttonScale, moveAreaTex);
		}
		else if (HUD.Instance.PlayerController.Director is SateliteKeyboardAndMouseControllerDirector)
		{
			SateliteKeyboardAndMouseControllerDirector sateliteKeyboardAndMouseControllerDirector = (SateliteKeyboardAndMouseControllerDirector)HUD.Instance.PlayerController.Director;
			InitMoveArea(ref _satelliteButtonArea, GUIPositionController.Instance.SatellitePercentLocation, (float)sateliteKeyboardAndMouseControllerDirector.SecondaryButton.BackgroundTexture.width * buttonScale, (float)sateliteKeyboardAndMouseControllerDirector.SecondaryButton.BackgroundTexture.height * buttonScale, moveAreaTex);
		}
	}

	private void InitMoveArea(ref MoveableGUIButton button, Vector2 location, float width, float height, Texture2D tex)
	{
		InitMoveArea(ref button, location, (int)width, (int)height, tex);
	}

	private void InitMoveArea(ref MoveableGUIButton button, Vector2 location, int width, int height, Texture2D tex)
	{
		button = new MoveableGUIButton();
		button.ButtonX = location.x * (float)Screen.width;
		button.ButtonY = location.y * (float)Screen.height;
		button.BackgroundTexture = tex;
		button.ButtonWidth = (float)width * PlayerGUI.Instance.SmallestRatio;
		button.ButtonHeight = (float)height * PlayerGUI.Instance.SmallestRatio;
		button.RenderDepth = 30f;
		button.InputDepth = 30f;
		PlayerGUI.Instance.AddComponent(button);
	}

	public void Close()
	{
		if (!(_blackBgObject != null) || !_blackBgObject.activeSelf)
		{
			return;
		}
		GUIPositionController.Instance.SaveToPlayerPrefs();
		_blackBgObject.SetActive(false);
		IsOpen = false;
		if (HUD.Instance.PlayerController != null)
		{
			SimpleControllerPerformer simpleControllerPerformer = (SimpleControllerPerformer)HUD.Instance.PlayerController.Performer;
			if (simpleControllerPerformer != null)
			{
				simpleControllerPerformer.Disabled = false;
			}
		}
		RemoveMoveableControls();
		HUD.Instance.OnShowControlMode(Preferences.Instance.CurrentControlMode);
	}

	public void CloseOnNextFrame()
	{
		_shouldClose = true;
	}

	private void RemoveMoveableControls()
	{
		if (_moveJoystickArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_moveJoystickArea);
		}
		if (_moveRadarArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_moveRadarArea);
		}
		if (_meleeButtonArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_meleeButtonArea);
		}
		if (_jumpButtonArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_jumpButtonArea);
		}
		if (_specialButtonArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_specialButtonArea);
		}
		if (_reloadButtonArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_reloadButtonArea);
		}
		if (_shootButtonArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_shootButtonArea);
		}
		if (_teamspeakArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_teamspeakArea);
		}
		if (_satelliteButtonArea != null)
		{
			PlayerGUI.Instance.RemoveComponent(_satelliteButtonArea);
		}
		PlayerGUI.Instance.RemoveComponent(_resetPositionButton);
		PlayerGUI.Instance.RemoveComponent(_acceptButton);
	}

	public void UpdateHUDElementPositions()
	{
		if (_shouldClose)
		{
			_shouldClose = false;
			Close();
			return;
		}
		if (_moveJoystickArea != null)
		{
			Vector2 moveJoystickPercentLocation = GUIPositionController.Instance.MoveJoystickPercentLocation;
			moveJoystickPercentLocation.x = _moveJoystickArea.ButtonX / (float)Screen.width;
			moveJoystickPercentLocation.y = _moveJoystickArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.MoveJoystickPercentLocation = moveJoystickPercentLocation;
		}
		if (_moveRadarArea != null)
		{
			Vector2 radarPercentLocation = GUIPositionController.Instance.RadarPercentLocation;
			radarPercentLocation.x = (_moveRadarArea.ButtonX - _moveRadarArea.ButtonWidth / 2f) / (float)Screen.width;
			radarPercentLocation.y = (_moveRadarArea.ButtonY - _moveRadarArea.ButtonHeight / 2f) / (float)Screen.height;
			GUIPositionController.Instance.RadarPercentLocation = radarPercentLocation;
		}
		if (_meleeButtonArea != null)
		{
			Vector2 meleeButtonPercentLocation = GUIPositionController.Instance.MeleeButtonPercentLocation;
			meleeButtonPercentLocation.x = _meleeButtonArea.ButtonX / (float)Screen.width;
			meleeButtonPercentLocation.y = _meleeButtonArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.MeleeButtonPercentLocation = meleeButtonPercentLocation;
		}
		if (_jumpButtonArea != null)
		{
			Vector2 jumpButtonPercentLocation = GUIPositionController.Instance.JumpButtonPercentLocation;
			jumpButtonPercentLocation.x = _jumpButtonArea.ButtonX / (float)Screen.width;
			jumpButtonPercentLocation.y = _jumpButtonArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.JumpButtonPercentLocation = jumpButtonPercentLocation;
		}
		if (_specialButtonArea != null)
		{
			Vector2 specialButtonPercentLocation = GUIPositionController.Instance.SpecialButtonPercentLocation;
			specialButtonPercentLocation.x = _specialButtonArea.ButtonX / (float)Screen.width;
			specialButtonPercentLocation.y = _specialButtonArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.SpecialButtonPercentLocation = specialButtonPercentLocation;
		}
		if (_reloadButtonArea != null)
		{
			Vector2 ammoBackdropPercentLocation = GUIPositionController.Instance.AmmoBackdropPercentLocation;
			ammoBackdropPercentLocation.x = _reloadButtonArea.ButtonX / (float)Screen.width;
			ammoBackdropPercentLocation.y = _reloadButtonArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.AmmoBackdropPercentLocation = ammoBackdropPercentLocation;
		}
		if (_shootButtonArea != null)
		{
			Vector2 gunButtonPercentLocation = GUIPositionController.Instance.GunButtonPercentLocation;
			gunButtonPercentLocation.x = _shootButtonArea.ButtonX / (float)Screen.width;
			gunButtonPercentLocation.y = _shootButtonArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.GunButtonPercentLocation = gunButtonPercentLocation;
		}
		if (_teamspeakArea != null)
		{
			Vector2 teamspeakButtonPercentLocation = GUIPositionController.Instance.TeamspeakButtonPercentLocation;
			teamspeakButtonPercentLocation.x = _teamspeakArea.ButtonX / (float)Screen.width;
			teamspeakButtonPercentLocation.y = _teamspeakArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.TeamspeakButtonPercentLocation = teamspeakButtonPercentLocation;
		}
		if (_satelliteButtonArea != null)
		{
			Vector2 satellitePercentLocation = GUIPositionController.Instance.SatellitePercentLocation;
			satellitePercentLocation.x = _satelliteButtonArea.ButtonX / (float)Screen.width;
			satellitePercentLocation.y = _satelliteButtonArea.ButtonY / (float)Screen.height;
			GUIPositionController.Instance.SatellitePercentLocation = satellitePercentLocation;
		}
		if (HUD.Instance.PlayerController != null)
		{
			SimpleControllerDirector simpleControllerDirector = (SimpleControllerDirector)HUD.Instance.PlayerController.Director;
			if (simpleControllerDirector != null)
			{
				simpleControllerDirector.PositionGUI();
			}
		}
	}
}
