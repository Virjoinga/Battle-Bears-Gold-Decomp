using System;
using UnityEngine;
using Utils;

public class Preferences : MonoBehaviour
{
	private static Preferences instance;

	[SerializeField]
	private Color _hudColor = Color.green;

	[SerializeField]
	private Color _hudButtonIconColor = Color.white;

	[SerializeField]
	private Color _hudPressedButtonColor = Color.green;

	[SerializeField]
	private Color _hudUnPressedButtonColor = new Color(0f, 1f, 0f, 0.5f);

	[SerializeField]
	private bool _useHudColorForUnPressedButton;

	private float sensitivity = 0.5f;

	private float buttonSize = 0.6f;

	private float _buttonSizeMinValue = 0.3f;

	private float _buttonSizePercentMod = 0.5f;

	private bool radarToggledOn = true;

	private bool zoomModeToggledOn = true;

	private bool _adsEnabled;

	private ShootMode shootMode = ShootMode.shootButton;

	private ControlMode currentControlMode;

	private GameMode gameMode;

	public static Preferences Instance
	{
		get
		{
			return instance;
		}
	}

	public Color HUDColor
	{
		get
		{
			return _hudColor;
		}
	}

	public Color HUDButtonIconColor
	{
		get
		{
			return _hudButtonIconColor;
		}
	}

	public Color HUDPressedButtonColor
	{
		get
		{
			return _hudPressedButtonColor;
		}
	}

	public Color HUDUnPressedButtonColor
	{
		get
		{
			return (!_useHudColorForUnPressedButton) ? _hudUnPressedButtonColor : _hudColor;
		}
	}

	public float Sensitivity
	{
		get
		{
			return sensitivity;
		}
		set
		{
			sensitivity = value;
			PrefsHelper.SetFloat("sensitivity", value);
		}
	}

	public float ButtonSize
	{
		get
		{
			return buttonSize * _buttonSizePercentMod + _buttonSizeMinValue;
		}
		set
		{
			buttonSize = value;
			PrefsHelper.SetFloat("buttonSize", value);
		}
	}

	public float ButtonSizePercent
	{
		get
		{
			return buttonSize;
		}
		set
		{
			ButtonSize = value;
		}
	}

	public bool RadarToggledOn
	{
		get
		{
			return radarToggledOn;
		}
		set
		{
			radarToggledOn = value;
			PrefsHelper.SetBool("radarToggledOn", value);
			if (HUD.Instance != null)
			{
				HUD.Instance.SetRadarEnabled(radarToggledOn);
			}
		}
	}

	public bool ZoomModeToggledOn
	{
		get
		{
			return zoomModeToggledOn;
		}
		set
		{
			zoomModeToggledOn = value;
			PrefsHelper.SetBool("zoomModeToggledOn", value);
		}
	}

	public bool AdsEnabled
	{
		get
		{
			return _adsEnabled;
		}
		set
		{
			_adsEnabled = value;
			PrefsHelper.SetBool("adsEnabled", value);
		}
	}

	public ShootMode CurrentShootMode
	{
		get
		{
			return shootMode;
		}
		set
		{
			shootMode = value;
			PrefsHelper.SetInt("shootButtonMode", (int)value);
			if (HUD.Instance != null)
			{
				HUD.Instance.OnShowControlMode(CurrentControlMode);
			}
		}
	}

	public ControlMode CurrentControlMode
	{
		get
		{
			return currentControlMode;
		}
		set
		{
			currentControlMode = value;
			PrefsHelper.SetInt("controlMode", (int)value);
			if (HUD.Instance != null)
			{
				HUD.Instance.OnShowControlMode(CurrentControlMode);
			}
		}
	}

	public GameMode CurrentGameMode
	{
		get
		{
			return gameMode;
		}
		set
		{
			gameMode = value;
			PrefsHelper.SetInt("gameMode", (int)value);
		}
	}

	public bool IsTeamMode
	{
		get
		{
			return gameMode.IsTeam();
		}
	}

	public string CurrentGameModeStr
	{
		get
		{
			string empty = string.Empty;
			switch (gameMode)
			{
			case GameMode.CTF:
				return "CTF";
			case GameMode.TB:
				return "FFA";
			case GameMode.FFA:
				return "EBFT";
			case GameMode.KOTH:
				return "KTH";
			case GameMode.ROYL:
				return "ROYL";
			default:
				throw new Exception("No string defined for Game Mode " + gameMode);
			}
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public void OnLoad()
	{
		sensitivity = PrefsHelper.GetFloat("sensitivity", 0.5f);
		buttonSize = PrefsHelper.GetFloat("buttonSize", 0.5f);
		radarToggledOn = PrefsHelper.GetBool("radarToggledOn", true);
		zoomModeToggledOn = PrefsHelper.GetBool("zoomModeToggledOn", true);
		shootMode = (ShootMode)PrefsHelper.GetInt("shootButtonMode", 1);
		currentControlMode = (ControlMode)PrefsHelper.GetInt("controlMode", 0);
		gameMode = (GameMode)PrefsHelper.GetInt("gameMode", 0);
		_adsEnabled = PrefsHelper.GetBool("adsEnabled", true);
	}
}
