using ExitGames.Client.Photon;
using UnityEngine;

public class CreateRoomController : MonoBehaviour
{
	private PrivateMatchController _privateMatchController;

	private Hashtable _properties;

	private string _roomName;

	private GUIButton _createBtn;

	[SerializeField]
	private LevelSelect _levels;

	[SerializeField]
	private ModeSelect _modes;

	private string _creationRoomFailerText = "Can't create Room, room with the same name already exists";

	private string _needRoomName = "Room name is empty, please name the room";

	private void Start()
	{
		UpdateLocalizedText();
		_properties = new Hashtable();
		_properties["customRoom"] = true;
		_properties["levelName"] = _levels.ChangeLevel(0);
		_modes.SetMode(GameMode.TB);
		_properties["gameMode"] = _modes.ChangeMode(0);
		_properties["password"] = string.Empty;
		_properties["client_version"] = ServiceManager.Instance.BuildIdentifier;
		_properties["platform"] = ServiceManager.Instance.Platform;
		_roomName = string.Empty;
	}

	private void UpdateLocalizedText()
	{
		_creationRoomFailerText = Language.Get("CREATE_MATCH_ROOM_ALREADY_EXISTS");
		_needRoomName = Language.Get("CREATE_MATCH_NO_ROOM_NAME");
	}

	private void SetController(PrivateMatchController controller)
	{
		_privateMatchController = controller;
	}

	private void OnGUIButtonClicked(GUIButton button)
	{
		switch (button.name)
		{
		case "create_previousMode":
			_properties["gameMode"] = _modes.ChangeMode(-1);
			break;
		case "create_nextMode":
			_properties["gameMode"] = _modes.ChangeMode(1);
			break;
		case "create_previousLevel":
		{
			string value = _levels.ChangeLevel(-1);
			_properties["levelName"] = value;
			break;
		}
		case "create_nextLevel":
		{
			string value2 = _levels.ChangeLevel(1);
			_properties["levelName"] = value2;
			break;
		}
		case "create_create":
			_createBtn = button;
			button.disable();
			if (string.IsNullOrEmpty(_roomName))
			{
				_privateMatchController.DisplayError(_needRoomName);
				button.enable();
			}
			else if (!_privateMatchController.CreateRoom(_roomName, _properties))
			{
				Debug.LogError("can't create, duplicate room name");
				_privateMatchController.DisplayError(_creationRoomFailerText);
				button.enable();
			}
			break;
		case "create_back":
			_privateMatchController.Close();
			break;
		case "create_name":
		case "create_password":
			break;
		default:
			Debug.Log("pressed " + button.name + " but don't have a case for it");
			break;
		}
	}

	private void OnInputFieldChanged(InputField inputField)
	{
		switch (inputField.name)
		{
		case "create_name":
			_roomName = inputField.actualString.ToLower();
			break;
		case "create_password":
			_properties["password"] = inputField.actualString.ToLower();
			break;
		}
	}

	private void ReEnableButtons()
	{
		if (_createBtn != null)
		{
			_createBtn.enable();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_privateMatchController.Close();
		}
	}
}
