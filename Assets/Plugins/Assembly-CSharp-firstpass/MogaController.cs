using UnityEngine;

public class MogaController : MonoBehaviour
{
	public int connection;

	public int buttonStart;

	public int buttonSelect;

	public int buttonA;

	public int buttonB;

	public int buttonX;

	public int buttonY;

	public int buttonL;

	public int buttonR;

	public float axisX;

	public float axisY;

	public float axisZ;

	public float axisRZ;

	public float axisLTrigger;

	public float axisRTrigger;

	public int buttonL2;

	public int buttonR2;

	public int buttonThumbL;

	public int buttonThumbR;

	public int dPadUp;

	public int dPadDown;

	public int dPadLeft;

	public int dPadRight;

	public int _lastConnection;

	public int _lastButtonStart;

	public int _lastButtonSelect;

	public int _lastButtonA;

	public int _lastButtonB;

	public int _lastButtonX;

	public int _lastButtonY;

	public int _lastButtonL;

	public int _lastButtonR;

	public float _lastAxisX;

	public float _lastAxisY;

	public float _lastAxisZ;

	public float _lastAxisRZ;

	public float _lastaxisLTrigger;

	public float _lastaxisRTrigger;

	public int _lastbuttonL2;

	public int _lastbuttonR2;

	public int _lastbuttonThumbL;

	public int _lastbuttonThumbR;

	public int _lastdPadUp;

	public int _lastdPadDown;

	public int _lastdPadLeft;

	public int _lastdPadRight;

	private static MogaController _instance;

	private Controller _mogaController;

	private bool _appFocused;

	public static MogaController Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("moga_controller");
				_instance = gameObject.AddComponent<MogaController>();
			}
			return _instance;
		}
		private set
		{
			_instance = value;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		_appFocused = focus;
		if (_mogaController != null)
		{
			if (_appFocused)
			{
				_mogaController.onResume();
			}
			else
			{
				_mogaController.onPause();
			}
		}
	}

	private void OnDestroy()
	{
		_mogaController.exit();
		_mogaController = null;
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(this);
		Debug.Log("MOGA controller instance created");
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public bool ButtonPressed(int controllerKeyCode)
	{
		int num = 0;
		int num2 = 0;
		switch (controllerKeyCode)
		{
		case 108:
			num = _lastButtonStart;
			num2 = buttonStart;
			break;
		case 109:
			num = _lastButtonSelect;
			num2 = buttonSelect;
			break;
		case 96:
			num = _lastButtonA;
			num2 = buttonA;
			break;
		case 97:
			num = _lastButtonB;
			num2 = buttonB;
			break;
		case 99:
			num = _lastButtonX;
			num2 = buttonX;
			break;
		case 100:
			num = _lastButtonY;
			num2 = buttonY;
			break;
		case 102:
			num = _lastButtonL;
			num2 = buttonL;
			break;
		case 103:
			num = _lastButtonR;
			num2 = buttonR;
			break;
		case 104:
			num = _lastbuttonL2;
			num2 = buttonL2;
			break;
		case 105:
			num = _lastbuttonR2;
			num2 = buttonR2;
			break;
		case 106:
			num = _lastbuttonThumbL;
			num2 = buttonThumbL;
			break;
		case 107:
			num = _lastbuttonThumbR;
			num2 = buttonThumbR;
			break;
		case 19:
			num = _lastdPadUp;
			num2 = dPadUp;
			break;
		case 20:
			num = _lastdPadDown;
			num2 = dPadDown;
			break;
		case 21:
			num = _lastdPadLeft;
			num2 = dPadLeft;
			break;
		case 22:
			num = _lastdPadRight;
			num2 = dPadRight;
			break;
		default:
			Debug.LogError("No MOGA input code exists for: " + controllerKeyCode);
			break;
		}
		return num == 1 && num2 == 0;
	}

	public bool ButtonReleased(int controllerKeyCode)
	{
		int num = 0;
		int num2 = 0;
		switch (controllerKeyCode)
		{
		case 108:
			num = _lastButtonStart;
			num2 = buttonStart;
			break;
		case 109:
			num = _lastButtonSelect;
			num2 = buttonSelect;
			break;
		case 96:
			num = _lastButtonA;
			num2 = buttonA;
			break;
		case 97:
			num = _lastButtonB;
			num2 = buttonB;
			break;
		case 99:
			num = _lastButtonX;
			num2 = buttonX;
			break;
		case 100:
			num = _lastButtonY;
			num2 = buttonY;
			break;
		case 102:
			num = _lastButtonL;
			num2 = buttonL;
			break;
		case 103:
			num = _lastButtonR;
			num2 = buttonR;
			break;
		case 104:
			num = _lastbuttonL2;
			num2 = buttonL2;
			break;
		case 105:
			num = _lastbuttonR2;
			num2 = buttonR2;
			break;
		case 106:
			num = _lastbuttonThumbL;
			num2 = buttonThumbL;
			break;
		case 107:
			num = _lastbuttonThumbR;
			num2 = buttonThumbR;
			break;
		case 19:
			num = _lastdPadUp;
			num2 = dPadUp;
			break;
		case 20:
			num = _lastdPadDown;
			num2 = dPadDown;
			break;
		case 21:
			num = _lastdPadLeft;
			num2 = dPadLeft;
			break;
		case 22:
			num = _lastdPadRight;
			num2 = dPadRight;
			break;
		default:
			Debug.LogError("No MOGA input code exists for: " + controllerKeyCode);
			break;
		}
		return num == 0 && num2 == 1;
	}

	public int GetControllerVersion()
	{
		return _mogaController.getState(4);
	}
}
