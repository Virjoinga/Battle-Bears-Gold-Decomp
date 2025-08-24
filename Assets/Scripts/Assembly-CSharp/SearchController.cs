using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchController : MonoBehaviour
{
	private enum TextPrompt
	{
		PASSWORD = 0,
		SEARCH = 1
	}

	private PrivateMatchController _controller;

	private List<RoomInfo> _rooms;

	private List<RoomPlate> _displayedRooms;

	private int _currentPage;

	private int _maxPage;

	private int _platesInPage;

	private RoomPlate _selectedRoom;

	private GameObject _currentTextPrompt;

	private string _password = string.Empty;

	private string _searchName = string.Empty;

	private TextPrompt _textPromptState;

	[SerializeField]
	private GameObject _roomPlatePrefab;

	[SerializeField]
	private float _topLimit;

	[SerializeField]
	private float _bottomLimit;

	[SerializeField]
	private float _plateSize;

	[SerializeField]
	private GameObject _textPromptPrefab;

	[SerializeField]
	private GUIButton _previousButton;

	[SerializeField]
	private GUIButton _nextButton;

	[SerializeField]
	private float _xPlatePosition;

	[SerializeField]
	private float _zPlatePosition = -43f;

	[SerializeField]
	private GUIButton _joinBtn;

	private string _incorrectPasswordError = "Incorrect password, please try again";

	private string _noSearchResaultsFound = "Could not find any rooms with the specified name";

	private string _failedJoinRoomError = "Game already started, could not join room.";

	private void Start()
	{
		UpdateLocalizedText();
		UpdateRooms();
		_displayedRooms = new List<RoomPlate>();
		_previousButton.disable();
		_nextButton.disable();
		_joinBtn.disable();
		if (_rooms.Count <= 0)
		{
			StartCoroutine(SearchForRooms());
			return;
		}
		Debug.Log("constructing pages");
		ConstructPages();
	}

	private void UpdateLocalizedText()
	{
		_incorrectPasswordError = Language.Get("MATCH_LOBBY_INCORRECT_PASSWORD_STATUS");
		_noSearchResaultsFound = Language.Get("MATCH_LOBBY_NO_SEARCH_RESULTS_STATUS");
		_failedJoinRoomError = Language.Get("MATCH_LOBBY_FAILED_TO_JOIN_STATUS");
	}

	private void SetController(PrivateMatchController controller)
	{
		_controller = controller;
	}

	private void OnGUIButtonClicked(GUIButton button)
	{
		if (button.name.Contains("room"))
		{
			if (_selectedRoom != null)
			{
				_selectedRoom.SetSelection(false);
			}
			int index = int.Parse(button.name.Split('_')[1]);
			_displayedRooms[index].SetSelection(true);
			_selectedRoom = _displayedRooms[index];
			_joinBtn.enable();
			return;
		}
		switch (button.name)
		{
		case "join":
			if (_selectedRoom != null && _selectedRoom.PasswordProtected)
			{
				_textPromptState = TextPrompt.PASSWORD;
				_password = string.Empty;
				CreateTextPrompt("password:");
			}
			else
			{
				JoinRoom();
			}
			break;
		case "close":
			_controller.Close();
			break;
		case "sort_name":
			SortName(false);
			break;
		case "sort_players":
			SortPlayers(true);
			break;
		case "sort_password":
			SortPassword(false);
			break;
		case "search":
			_textPromptState = TextPrompt.SEARCH;
			_searchName = string.Empty;
			CreateTextPrompt("search arena name:");
			break;
		case "refresh":
			UpdateRooms();
			ConstructPages();
			break;
		case "ok":
			if (_textPromptState == TextPrompt.PASSWORD)
			{
				if (_password == (string)_selectedRoom.Room.customProperties["password"])
				{
					JoinRoom();
					RemoveTextPrompt();
				}
				else
				{
					Debug.LogError("Incorrect password, try again!");
					_joinBtn.enable();
					_controller.DisplayError(_incorrectPasswordError);
				}
				break;
			}
			UpdateRooms();
			SearchRoomName(_searchName);
			if (_rooms.Count <= 0)
			{
				_joinBtn.enable();
				_controller.DisplayError(_noSearchResaultsFound);
				RemoveTextPrompt();
			}
			else
			{
				SetPage(0);
				RemoveTextPrompt();
			}
			break;
		case "cancel":
			RemoveTextPrompt();
			break;
		case "nextPage":
			_currentPage++;
			SetPage(_currentPage);
			break;
		case "previousPage":
			_currentPage--;
			SetPage(_currentPage);
			break;
		}
	}

	private void OnInputFieldChanged(InputField field)
	{
		switch (field.name)
		{
		case "textField":
			if (_textPromptState == TextPrompt.PASSWORD)
			{
				_password = field.actualString.ToLower();
			}
			else
			{
				_searchName = field.actualString.ToLower();
			}
			break;
		}
	}

	private IEnumerator SearchForRooms()
	{
		while (_rooms.Count <= 0)
		{
			yield return new WaitForSeconds(1f);
			UpdateRooms();
		}
		UpdateRooms();
		ConstructPages();
	}

	private void ConstructPages()
	{
		_selectedRoom = null;
		_joinBtn.disable();
		for (int i = 0; i < _displayedRooms.Count; i++)
		{
			Object.Destroy(_displayedRooms[i].gameObject);
		}
		_displayedRooms.Clear();
		_platesInPage = (int)((_topLimit - _bottomLimit) / _plateSize);
		if (_platesInPage != _rooms.Count)
		{
			_maxPage = _rooms.Count / _platesInPage;
		}
		else
		{
			_maxPage = _rooms.Count / _platesInPage - 1;
		}
		_currentPage = 0;
		while (_displayedRooms.Count < _platesInPage)
		{
			GameObject gameObject = Object.Instantiate(_roomPlatePrefab) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = new Vector3(_xPlatePosition, _topLimit - (float)_displayedRooms.Count * _plateSize, _zPlatePosition);
			RoomPlate component = gameObject.GetComponent<RoomPlate>();
			if (component != null)
			{
				gameObject.name = "room_" + _displayedRooms.Count;
				_displayedRooms.Add(component);
			}
			else
			{
				Debug.LogError("RoomPlate not found, make sure its attached");
			}
		}
		SetPage(0);
	}

	private void SetPage(int page)
	{
		_currentPage = page;
		_previousButton.enable();
		_nextButton.enable();
		if (_currentPage <= 0)
		{
			_previousButton.disable();
		}
		if (_currentPage >= _maxPage)
		{
			_nextButton.disable();
		}
		for (int i = 0; i < _displayedRooms.Count; i++)
		{
			if (_currentPage * _platesInPage + i < _rooms.Count)
			{
				_displayedRooms[i].gameObject.SetActive(true);
				_displayedRooms[i].ConstructRoomPlate(_rooms[_currentPage * _platesInPage + i]);
			}
			else
			{
				_displayedRooms[i].gameObject.SetActive(false);
			}
		}
	}

	private void JoinRoom()
	{
		if (_selectedRoom == null || _selectedRoom.Room == null)
		{
			Debug.LogError("No room is selected!");
			return;
		}
		_controller.JoinRoom(_selectedRoom.Room);
		_joinBtn.disable();
	}

	private void UpdateRooms()
	{
		if (_rooms != null && _rooms.Count > 0)
		{
			_rooms.Clear();
		}
		_rooms = new List<RoomInfo>();
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.customProperties.ContainsKey("customRoom") && roomInfo.customProperties.ContainsKey("client_version") && (string)roomInfo.customProperties["client_version"] == ServiceManager.Instance.BuildIdentifier && roomInfo.open && (ServiceManager.Instance.PrivateMatchPlatforms.Equals(string.Empty) || (roomInfo.customProperties.ContainsKey("platform") && ServiceManager.Instance.PrivateMatchPlatforms.Contains((string)roomInfo.customProperties["platform"]))))
			{
				_rooms.Add(roomInfo);
			}
		}
	}

	private void SearchRoomName(string name)
	{
		List<RoomInfo> list = new List<RoomInfo>();
		foreach (RoomInfo room in _rooms)
		{
			if (room.name.Contains(name))
			{
				list.Add(room);
			}
		}
		_rooms = list;
	}

	private void SortName(bool ascending)
	{
		_rooms.Sort((RoomInfo x, RoomInfo y) => ascending ? (-string.Compare(x.name, y.name)) : string.Compare(x.name, y.name));
		SetPage(0);
	}

	private void SortPlayers(bool ascending)
	{
		_rooms.Sort(delegate(RoomInfo x, RoomInfo y)
		{
			if (x.playerCount > y.playerCount)
			{
				return (!ascending) ? 1 : (-1);
			}
			return (x.playerCount < y.playerCount) ? (ascending ? 1 : (-1)) : 0;
		});
		SetPage(0);
	}

	private void SortPassword(bool ascending)
	{
		_rooms.Sort(delegate(RoomInfo x, RoomInfo y)
		{
			if (x.customProperties.ContainsKey("password") && !string.IsNullOrEmpty((string)x.customProperties["password"]))
			{
				if (y.customProperties.ContainsKey("password") && !string.IsNullOrEmpty((string)y.customProperties["password"]))
				{
					Debug.Log("equal");
					return 0;
				}
				Debug.Log("string; " + (string)x.customProperties["password"]);
				return ascending ? 1 : (-1);
			}
			if (y.customProperties.ContainsKey("password") && !string.IsNullOrEmpty((string)y.customProperties["password"]))
			{
				Debug.Log("better");
				return (!ascending) ? 1 : (-1);
			}
			Debug.Log("equal");
			return 0;
		});
		SetPage(0);
	}

	private void CreateTextPrompt(string promptQuestion)
	{
		if (_currentTextPrompt != null)
		{
			if (_currentTextPrompt.GetComponent<Animation>() != null && (bool)_currentTextPrompt.GetComponent<Animation>().GetClip("out"))
			{
				_currentTextPrompt.GetComponent<Animation>().Play("out");
			}
			else
			{
				Object.Destroy(_currentTextPrompt);
			}
		}
		_currentTextPrompt = Object.Instantiate(_textPromptPrefab, _textPromptPrefab.transform.position, _textPromptPrefab.transform.rotation) as GameObject;
		_currentTextPrompt.transform.parent = base.transform;
		_currentTextPrompt.name = promptQuestion;
	}

	private void RemoveTextPrompt()
	{
		if (_currentTextPrompt != null && _currentTextPrompt.GetComponent<Animation>() != null && _currentTextPrompt.GetComponent<Animation>().GetClip("out") != null)
		{
			_currentTextPrompt.GetComponent<Animation>().Play("out");
		}
		else
		{
			Object.Destroy(_currentTextPrompt);
		}
	}

	private void OnPhotonJoinRoomFailed()
	{
		_joinBtn.enable();
		_controller.DisplayError(_failedJoinRoomError);
		UpdateRooms();
		ConstructPages();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_controller.Close();
		}
	}
}
