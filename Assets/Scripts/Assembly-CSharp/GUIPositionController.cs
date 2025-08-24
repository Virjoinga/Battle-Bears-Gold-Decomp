using UnityEngine;

public class GUIPositionController
{
	private static GUIPositionController _instance;

	private Vector2 _jumpButtonPercentLocation;

	private Vector2 _meleeButtonPercentLocation;

	private Vector2 _specialButtonPercentLocation;

	private Vector2 _teamspeakButtonPercentLocation;

	private Vector2 _scoreBackdropPercentLocation;

	private Vector2 _redScoreBackgroundPercentLocation;

	private Vector2 _blueScoreBackgroundPercentLocation;

	private Vector2 _optionsButtonFFAPercentLocation;

	private Vector2 _optionsButtonPercentLocation;

	private Vector2 _redScoreTextPercentLocation;

	private Vector2 _blueScoreTextPercentLocation;

	private Vector2 _ammoBackdropPercentLocation;

	private Vector2 _ammoIconPercentLocation;

	private Vector2 _ammoTextPercentLocation;

	private Vector2 _ammoSlashPercentLocation;

	private Vector2 _clipTextPercentLocation;

	private Vector2 _swapWeaponAreaPercentLocation;

	private Vector2 _swapUpArrowPercentLocation;

	private Vector2 _swapDownArrowPercentLocation;

	private Vector2 _healthBarAreaPercentLocation;

	private Vector2 _healthTextPercentLocation;

	private Vector2 _moveJoystickPercentLocation;

	private Vector2 _gunButtonPercentLocation;

	private Vector2 _radarPercentLocation;

	private Vector2 _satelliteButtonPercentLocation;

	public static GUIPositionController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GUIPositionController();
			}
			return _instance;
		}
	}

	public Vector2 AmmoBackdropPercentLocation
	{
		get
		{
			return _ammoBackdropPercentLocation;
		}
		set
		{
			_ammoBackdropPercentLocation = value;
		}
	}

	public Vector2 AmmoIconPercentLocation
	{
		get
		{
			return _ammoIconPercentLocation;
		}
		set
		{
			_ammoIconPercentLocation = value;
		}
	}

	public Vector2 AmmoTextPercentLocation
	{
		get
		{
			return _ammoTextPercentLocation;
		}
		set
		{
			_ammoTextPercentLocation = value;
		}
	}

	public Vector2 AmmoSlashPercentLocation
	{
		get
		{
			return _ammoSlashPercentLocation;
		}
		set
		{
			_ammoSlashPercentLocation = value;
		}
	}

	public Vector2 BlueScoreBackgroundPercentLocation
	{
		get
		{
			return _blueScoreBackgroundPercentLocation;
		}
		set
		{
			_blueScoreBackgroundPercentLocation = value;
		}
	}

	public Vector2 BlueScoreTextPercentLocation
	{
		get
		{
			return _blueScoreTextPercentLocation;
		}
		set
		{
			_blueScoreTextPercentLocation = value;
		}
	}

	public Vector2 ClipTextPercentLocation
	{
		get
		{
			return _clipTextPercentLocation;
		}
		set
		{
			_clipTextPercentLocation = value;
		}
	}

	public Vector2 HealthBarAreaPercentLocation
	{
		get
		{
			return _healthBarAreaPercentLocation;
		}
		set
		{
			_healthBarAreaPercentLocation = value;
		}
	}

	public Vector2 HealthTextPercentLocation
	{
		get
		{
			return _healthTextPercentLocation;
		}
		set
		{
			_healthTextPercentLocation = value;
		}
	}

	public Vector2 JumpButtonPercentLocation
	{
		get
		{
			return _jumpButtonPercentLocation;
		}
		set
		{
			_jumpButtonPercentLocation = value;
		}
	}

	public Vector2 MeleeButtonPercentLocation
	{
		get
		{
			return _meleeButtonPercentLocation;
		}
		set
		{
			_meleeButtonPercentLocation = value;
		}
	}

	public Vector2 OptionsButtonFFAPercentLocation
	{
		get
		{
			return _optionsButtonFFAPercentLocation;
		}
		set
		{
			_optionsButtonFFAPercentLocation = value;
		}
	}

	public Vector2 OptionsButtonPercentLocation
	{
		get
		{
			return _optionsButtonPercentLocation;
		}
		set
		{
			_optionsButtonPercentLocation = value;
		}
	}

	public Vector2 RedScoreBackgroundPercentLocation
	{
		get
		{
			return _redScoreBackgroundPercentLocation;
		}
		set
		{
			_redScoreBackgroundPercentLocation = value;
		}
	}

	public Vector2 RedScoreTextPercentLocation
	{
		get
		{
			return _redScoreTextPercentLocation;
		}
		set
		{
			_redScoreTextPercentLocation = value;
		}
	}

	public Vector2 ScoreBackdropPercentLocation
	{
		get
		{
			return _scoreBackdropPercentLocation;
		}
		set
		{
			_scoreBackdropPercentLocation = value;
		}
	}

	public Vector2 SpecialButtonPercentLocation
	{
		get
		{
			return _specialButtonPercentLocation;
		}
		set
		{
			_specialButtonPercentLocation = value;
		}
	}

	public Vector2 SwapDownArrowPercentLocation
	{
		get
		{
			return _swapDownArrowPercentLocation;
		}
		set
		{
			_swapDownArrowPercentLocation = value;
		}
	}

	public Vector2 SwapUpArrowPercentLocation
	{
		get
		{
			return _swapUpArrowPercentLocation;
		}
		set
		{
			_swapUpArrowPercentLocation = value;
		}
	}

	public Vector2 SwapWeaponAreaPercentLocation
	{
		get
		{
			return _swapWeaponAreaPercentLocation;
		}
		set
		{
			_swapWeaponAreaPercentLocation = value;
		}
	}

	public Vector2 TeamspeakButtonPercentLocation
	{
		get
		{
			return _teamspeakButtonPercentLocation;
		}
		set
		{
			_teamspeakButtonPercentLocation = value;
		}
	}

	public Vector2 GunButtonPercentLocation
	{
		get
		{
			return _gunButtonPercentLocation;
		}
		set
		{
			_gunButtonPercentLocation = value;
		}
	}

	public Vector2 MoveJoystickPercentLocation
	{
		get
		{
			return _moveJoystickPercentLocation;
		}
		set
		{
			_moveJoystickPercentLocation = value;
		}
	}

	public Vector2 RadarPercentLocation
	{
		get
		{
			return _radarPercentLocation;
		}
		set
		{
			_radarPercentLocation = value;
		}
	}

	public Vector2 SatellitePercentLocation
	{
		get
		{
			return _satelliteButtonPercentLocation;
		}
		set
		{
			_satelliteButtonPercentLocation = value;
		}
	}

	public GUIPositionController()
	{
		SetToDefaultPositions();
		if (PlayerPrefs.GetString("jump").Equals(string.Empty))
		{
			SaveToPlayerPrefs();
		}
	}

	public void SetToDefaultPositions()
	{
		_jumpButtonPercentLocation = new Vector2(0.91f, 0.83f);
		_meleeButtonPercentLocation = new Vector2(0.74f, 0.9f);
		_specialButtonPercentLocation = new Vector2(0.91f, 0.63f);
		_teamspeakButtonPercentLocation = new Vector2(0.09f, 0.17f);
		_scoreBackdropPercentLocation = new Vector2(0.5f, 0.03f);
		_redScoreBackgroundPercentLocation = new Vector2(-0.055f, 0f);
		_blueScoreBackgroundPercentLocation = new Vector2(0.055f, 0f);
		_optionsButtonFFAPercentLocation = new Vector2(0f, 0.1f);
		_optionsButtonPercentLocation = new Vector2(0f, 0f);
		_redScoreTextPercentLocation = new Vector2(-0.045f, 0f);
		_blueScoreTextPercentLocation = new Vector2(0.045f, 0f);
		_ammoBackdropPercentLocation = new Vector2(0.88f, 0.25f);
		_ammoIconPercentLocation = new Vector2(-0.044f, -0.005f);
		_ammoTextPercentLocation = new Vector2(-0.006f, -0.006f);
		_ammoSlashPercentLocation = new Vector2(0.025f, -0.015f);
		_clipTextPercentLocation = new Vector2(0.045f, -0.015f);
		_swapWeaponAreaPercentLocation = new Vector2(0.885f, 0.08f);
		_swapUpArrowPercentLocation = new Vector2(-0.07f, -0.02f);
		_swapDownArrowPercentLocation = new Vector2(0.07f, 0.02f);
		_healthBarAreaPercentLocation = new Vector2(0.07f, 0.007f);
		_healthTextPercentLocation = new Vector2(0f, 0f);
		_moveJoystickPercentLocation = new Vector2(0.16f, 0.78f);
		_gunButtonPercentLocation = new Vector2(0.73f, 0.67f);
		_radarPercentLocation = new Vector2(0.02f, 0.35f);
		_satelliteButtonPercentLocation = new Vector2(0.75f, 0.4f);
	}

	public void SaveToPlayerPrefs()
	{
		SaveVector2ToPrefs("jump", _jumpButtonPercentLocation);
		SaveVector2ToPrefs("melee", _meleeButtonPercentLocation);
		SaveVector2ToPrefs("special", _specialButtonPercentLocation);
		SaveVector2ToPrefs("teamspeak", _teamspeakButtonPercentLocation);
		SaveVector2ToPrefs("scoreBack", _scoreBackdropPercentLocation);
		SaveVector2ToPrefs("ammoBackdrop", _ammoBackdropPercentLocation);
		SaveVector2ToPrefs("swapWeaponArea", _swapWeaponAreaPercentLocation);
		SaveVector2ToPrefs("healthArea", _healthBarAreaPercentLocation);
		SaveVector2ToPrefs("moveJoystick", _moveJoystickPercentLocation);
		SaveVector2ToPrefs("gunButton", _gunButtonPercentLocation);
		SaveVector2ToPrefs("radarPosition", _radarPercentLocation);
		SaveVector2ToPrefs("satelliteButtonPosition", _satelliteButtonPercentLocation);
	}

	private void SaveVector2ToPrefs(string name, Vector2 vec)
	{
		PlayerPrefs.SetString(name, vec.x + ":" + vec.y);
	}

	public void LoadFromPlayerPrefs()
	{
		_jumpButtonPercentLocation = LoadVector2FromPrefs("jump");
		_meleeButtonPercentLocation = LoadVector2FromPrefs("melee");
		_specialButtonPercentLocation = LoadVector2FromPrefs("special");
		_teamspeakButtonPercentLocation = LoadVector2FromPrefs("teamspeak");
		_scoreBackdropPercentLocation = LoadVector2FromPrefs("scoreBack");
		_ammoBackdropPercentLocation = LoadVector2FromPrefs("ammoBackdrop");
		_swapWeaponAreaPercentLocation = LoadVector2FromPrefs("swapWeaponArea");
		_healthBarAreaPercentLocation = LoadVector2FromPrefs("healthArea");
		_moveJoystickPercentLocation = LoadVector2FromPrefs("moveJoystick");
		_gunButtonPercentLocation = LoadVector2FromPrefs("gunButton");
		_radarPercentLocation = LoadVector2FromPrefs("radarPosition");
		Vector2 satelliteButtonPercentLocation = _satelliteButtonPercentLocation;
		_satelliteButtonPercentLocation = LoadVector2FromPrefs("satelliteButtonPosition");
		if (_satelliteButtonPercentLocation == Vector2.zero)
		{
			_satelliteButtonPercentLocation = satelliteButtonPercentLocation;
		}
	}

	private Vector2 LoadVector2FromPrefs(string name)
	{
		Vector2 result = Vector2.zero;
		string[] array = PlayerPrefs.GetString(name).Split(':');
		if (array.Length > 1)
		{
			string s = array[0];
			string s2 = array[1];
			result = new Vector2(float.Parse(s), float.Parse(s2));
		}
		return result;
	}

	public void ResetCustomPositions()
	{
		SetToDefaultPositions();
		SaveToPlayerPrefs();
	}
}
