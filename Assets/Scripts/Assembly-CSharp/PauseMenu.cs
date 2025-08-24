using System;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using UnityEngine;

public class PauseMenu : Popup
{
	public SliderHelper musicScroller;

	public SliderHelper sfxScroller;

	public SliderHelper sensitivityScroller;

	public SliderHelper buttonSizeScroller;

	public SliderHelper graphicsScroller;

	private Camera guiCamera;

	public LayerMask layerMask;

	public float percentNearEdge = 0.1f;

	public AudioClip inSound;

	public AudioClip outSound;

	public GUIButton radarOnButton;

	public GUIButton radarOffButton;

	public GUIButton zoomOnButton;

	public GUIButton zoomOffButton;

	public GUIButton doubletapButtonMobile;

	public GUIButton shootButtonMobile;

	public GUIButton doubletapButtonWindows;

	public GUIButton shootButtonWindows;

	public GUIButton keyboardAndMouseButtonWindows;

	private GUIButton doubletapButton;

	private GUIButton shootButton;

	private GUIButton keyboardAndMouseButton;

	public GUIButton customControlButton;

	public GUIButton defaultControlButton;

	public GameObject mobileButtonGroup;

	public GameObject windowsButtonGroup;

	[SerializeField]
	private TextMesh _adsEnabledText;

	private QualitySetting startingQualitySetting;

	public static QualitySetting chosenQualitySetting;

	public GameObject reloadPrompt;

	private string _loggedInFormat;

	private string _playerNamePrefix;

	protected override void Start()
	{
		base.Start();
		UpdateLocalizedText();
		doubletapButton = doubletapButtonMobile;
		shootButton = shootButtonMobile;
		windowsButtonGroup.SetActive(false);
		mobileButtonGroup.SetActive(true);
		if (inSound != null)
		{
			AudioSource.PlayClipAtPoint(inSound, Vector3.zero);
		}
		startingQualitySetting = BBRQuality.Current;
		chosenQualitySetting = BBRQuality.Current;
		musicScroller.SetIndicatorToPercent(SoundManager.Instance.getMusicVolume());
		musicScroller.SetOperationMethod(delegate(float a)
		{
			EventTracker.TrackEvent(new MusicVolumeChangedSchema(new MusicVolumeParameter(a)));
			SoundManager.Instance.setMusicVolume(a);
		});
		sfxScroller.SetIndicatorToPercent(SoundManager.Instance.getEffectsVolume());
		sfxScroller.SetOperationMethod(delegate(float a)
		{
			EventTracker.TrackEvent(new SFXVolumeChangedSchema(new SFXVolumeParameter(a)));
			SoundManager.Instance.setEffectsVolume(a);
		});
		sensitivityScroller.SetIndicatorToPercent(Preferences.Instance.Sensitivity);
		sensitivityScroller.SetOperationMethod(delegate(float a)
		{
			EventTracker.TrackEvent(new SensitivityChangedSchema(new AimingSensitivityParameter(a)));
			Preferences.Instance.Sensitivity = a;
		});
		if (buttonSizeScroller != null)
		{
			buttonSizeScroller.SetIndicatorToPercent(Preferences.Instance.ButtonSizePercent);
			buttonSizeScroller.SetOperationMethod(delegate(float a)
			{
				EventTracker.TrackEvent(new ButtonSizeSchema(new ButtonSizeParameter(a)));
				Preferences.Instance.ButtonSize = a;
			});
		}
		if (graphicsScroller != null)
		{
			graphicsScroller.SetIndicatorToPercent((float)BBRQuality.Current / (float)(BBRQuality.SettingsCount - 1));
			graphicsScroller.SetOperationMethod(delegate(float a)
			{
				chosenQualitySetting = (QualitySetting)Mathf.RoundToInt(a * (float)(BBRQuality.SettingsCount - 1));
			});
		}
		guiCamera = base.transform.parent.GetComponentInChildren(typeof(Camera)) as Camera;
		if (base.transform.Find("login") != null && !ServiceManager.Instance.GetStats().guest)
		{
			base.transform.Find("login").gameObject.SetActiveRecursively(false);
			TextMesh component = base.transform.Find("logout/logoutName").GetComponent<TextMesh>();
			component.text = string.Format(_loggedInFormat, LoginManager.lastUserLoggedIn);
		}
		else if (base.transform.Find("logout") != null)
		{
			base.transform.Find("logout").gameObject.SetActiveRecursively(false);
		}
		_playerNamePrefix = Language.Get("PLAYER_NAME_PREFIX");
		UpdateRadarToggleButtons();
		UpdateZoomModeButtons();
		UpdateDoubleTapModeButtons();
		UpdateControlButtons();
		if (_adsEnabledText != null)
		{
			_adsEnabledText.text = ((!Preferences.Instance.AdsEnabled) ? "ads off" : "ads on");
		}
	}

	private void UpdateLocalizedText()
	{
		_loggedInFormat = Language.Get("OPTIONS_POPUP_LOGGED_IN_AS");
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		switch (b.name)
		{
		case "backBtn":
			if (outSound != null)
			{
				AudioSource.PlayClipAtPoint(outSound, Vector3.zero);
			}
			if (startingQualitySetting != chosenQualitySetting)
			{
				UnityEngine.Object.Instantiate(reloadPrompt);
			}
			else
			{
				OnClose();
			}
			break;
		case "tutorialControl_Button":
			if (HUD.Instance != null)
			{
				MogaPopUpHandler.ShowTutorial();
			}
			if (outSound != null)
			{
				AudioSource.PlayClipAtPoint(outSound, Vector3.zero);
			}
			OnClose();
			break;
		case "exitButton":
		{
			Tutorial tutorial = UnityEngine.Object.FindObjectOfType(typeof(Tutorial)) as Tutorial;
			if (tutorial != null)
			{
				tutorial.QuittingEarly();
			}
			if (!LoginManager.offlineMode)
			{
				if (Application.loadedLevelName != "Tutorial")
				{
					EventTracker.TrackEvent(MatchEventsHelper.MatchExited(false));
					PhotonManager.Instance.Leave();
					ServiceManager.Instance.LogGameLeft("user_quit");
				}
				Application.LoadLevel("MainMenu");
			}
			else
			{
				Application.LoadLevel("Login");
			}
			break;
		}
		case "login":
			LoginManager.Instance.OnShowCreateAccountMenu(disabledGUIController);
			break;
		case "logout":
			PlayerPrefs.DeleteKey("username");
			Application.LoadLevel("Login");
			break;
		case "radarOn":
			Preferences.Instance.RadarToggledOn = true;
			UpdateRadarToggleButtons();
			break;
		case "radarOff":
			Preferences.Instance.RadarToggledOn = false;
			UpdateRadarToggleButtons();
			break;
		case "zoomOn":
			Preferences.Instance.ZoomModeToggledOn = true;
			UpdateZoomModeButtons();
			break;
		case "zoomOff":
			Preferences.Instance.ZoomModeToggledOn = false;
			UpdateZoomModeButtons();
			break;
		case "buttonToShoot":
			Preferences.Instance.CurrentShootMode = ShootMode.shootButton;
			EventTracker.TrackEvent(new ShootModeChangedSchema(new ShootModeParameter(ShootMode.shootButton)));
			UpdateDoubleTapModeButtons();
			break;
		case "doubleTapToShoot":
			Preferences.Instance.CurrentShootMode = ShootMode.doubleTap;
			EventTracker.TrackEvent(new ShootModeChangedSchema(new ShootModeParameter(ShootMode.doubleTap)));
			UpdateDoubleTapModeButtons();
			break;
		case "keyboardAndMouse":
			Preferences.Instance.CurrentShootMode = ShootMode.keyboardAndMouse;
			UpdateDoubleTapModeButtons();
			break;
		case "custom":
			Preferences.Instance.CurrentControlMode = ControlMode.custom;
			UpdateControlButtons();
			OnClose();
			CustomLayoutController.Instance.Show();
			EventTracker.TrackEvent(new ControlSchemeChangedSchema(new ControlSchemeParameter(ControlSchemeParameter.Scheme.CUSTOM)));
			break;
		case "defaultControl":
			Preferences.Instance.CurrentControlMode = ControlMode.defaultHud;
			UpdateControlButtons();
			EventTracker.TrackEvent(new ControlSchemeChangedSchema(new ControlSchemeParameter(ControlSchemeParameter.Scheme.DEFAULT)));
			break;
		case "changeNickname":
		{
			PlayerNicknamePopupManager instance = PlayerNicknamePopupManager.Instance;
			instance.NicknameCanceled = (Action)Delegate.Remove(instance.NicknameCanceled, new Action(OnUserNamePromptCancelled));
			PlayerNicknamePopupManager instance2 = PlayerNicknamePopupManager.Instance;
			instance2.NicknameCanceled = (Action)Delegate.Combine(instance2.NicknameCanceled, new Action(OnUserNamePromptCancelled));
			PlayerNicknamePopupManager instance3 = PlayerNicknamePopupManager.Instance;
			instance3.NicknameSaved = (Action<string>)Delegate.Remove(instance3.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
			PlayerNicknamePopupManager instance4 = PlayerNicknamePopupManager.Instance;
			instance4.NicknameSaved = (Action<string>)Delegate.Combine(instance4.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
			PlayerNicknamePopupManager.Instance.ShowNicknamePopupWithInitialName(Bootloader.Instance.socialName);
			break;
		}
		case "adsButton":
		{
			bool flag = !Preferences.Instance.AdsEnabled;
			Preferences.Instance.AdsEnabled = flag;
			_adsEnabledText.text = ((!flag) ? "ads off" : "ads on");
			break;
		}
		}
	}

	private void UpdateRadarToggleButtons()
	{
		if (!(radarOnButton == null) && !(radarOffButton == null))
		{
			if (Preferences.Instance.RadarToggledOn)
			{
				radarOnButton.disable();
				radarOffButton.enable();
			}
			else if (!Preferences.Instance.RadarToggledOn)
			{
				radarOnButton.enable();
				radarOffButton.disable();
			}
		}
	}

	private void UpdateZoomModeButtons()
	{
		if (!(zoomOnButton == null) && !(zoomOffButton == null))
		{
			if (Preferences.Instance.ZoomModeToggledOn)
			{
				zoomOnButton.disable();
				zoomOffButton.enable();
			}
			else if (!Preferences.Instance.ZoomModeToggledOn)
			{
				zoomOnButton.enable();
				zoomOffButton.disable();
			}
		}
	}

	private void UpdateDoubleTapModeButtons()
	{
		if (shootButton != null)
		{
			shootButton.enable();
		}
		if (doubletapButton != null)
		{
			doubletapButton.enable();
		}
		if (keyboardAndMouseButton != null)
		{
			keyboardAndMouseButton.enable();
		}
		if (Preferences.Instance.CurrentShootMode == ShootMode.shootButton && shootButton != null)
		{
			shootButton.disable();
		}
		else if (Preferences.Instance.CurrentShootMode == ShootMode.doubleTap && doubletapButton != null)
		{
			doubletapButton.disable();
		}
		else if (Preferences.Instance.CurrentShootMode == ShootMode.keyboardAndMouse && keyboardAndMouseButton != null)
		{
			keyboardAndMouseButton.disable();
		}
	}

	private void UpdateControlButtons()
	{
		if (customControlButton != null && defaultControlButton != null)
		{
			if (Preferences.Instance.CurrentControlMode == ControlMode.custom)
			{
				customControlButton.showDisableButAllowClicks();
				defaultControlButton.enable();
			}
			else
			{
				customControlButton.enable();
				defaultControlButton.disable();
			}
			if (HUD.Instance != null)
			{
				HUD.Instance.OnShowControlMode(Preferences.Instance.CurrentControlMode);
			}
		}
	}

	private void OnUserNamePromptFinished(string username)
	{
		PlayerNicknamePopupManager instance = PlayerNicknamePopupManager.Instance;
		instance.NicknameCanceled = (Action)Delegate.Remove(instance.NicknameCanceled, new Action(OnUserNamePromptCancelled));
		PlayerNicknamePopupManager instance2 = PlayerNicknamePopupManager.Instance;
		instance2.NicknameSaved = (Action<string>)Delegate.Remove(instance2.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
		Bootloader.Instance.socialName = username;
		PlayerPrefs.SetString("socialName", username);
		if (Bootloader.Instance.socialName == string.Empty)
		{
			PlayerPrefs.SetString("socialName", _playerNamePrefix + ServiceManager.Instance.GetStats().pid);
		}
	}

	private void OnUserNamePromptCancelled()
	{
		PlayerNicknamePopupManager instance = PlayerNicknamePopupManager.Instance;
		instance.NicknameCanceled = (Action)Delegate.Remove(instance.NicknameCanceled, new Action(OnUserNamePromptCancelled));
		PlayerNicknamePopupManager instance2 = PlayerNicknamePopupManager.Instance;
		instance2.NicknameSaved = (Action<string>)Delegate.Remove(instance2.NicknameSaved, new Action<string>(OnUserNamePromptFinished));
		string @string = PlayerPrefs.GetString("socialName", string.Empty);
		if (Bootloader.Instance.socialName == string.Empty && @string == string.Empty)
		{
			PlayerPrefs.SetString("socialName", _playerNamePrefix + ServiceManager.Instance.GetStats().pid);
		}
	}

	private void Update()
	{
		if (Input.touchCount <= 0)
		{
			return;
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			Ray ray = guiCamera.ScreenPointToRay(Input.GetTouch(i).position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 1000f, layerMask))
			{
				Collider collider = hitInfo.collider;
				SliderHelper component = collider.gameObject.GetComponent<SliderHelper>();
				if (component != null)
				{
					component.SetIndicatorToWorldPos(hitInfo.point.x);
				}
			}
		}
	}

	public void OnDestroy()
	{
		PlayerPrefs.Save();
		if (HUD.Instance != null)
		{
			HUD.Instance.OnShowControlMode(Preferences.Instance.CurrentControlMode);
		}
		if (ClosingCallback != null)
		{
			ClosingCallback();
		}
	}
}
