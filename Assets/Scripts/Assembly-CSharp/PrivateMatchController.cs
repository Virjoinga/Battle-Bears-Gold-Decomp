using System;
using System.Collections;
using System.Runtime.InteropServices;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using ExitGames.Client.Photon;
using UnityEngine;

public class PrivateMatchController : Popup
{
	public enum PrivateMatchState
	{
		PROMPT = 0,
		SEARCH = 1,
		CREATE = 2,
		LOBBY = 3
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CustomProperties
	{
		public const string CUSTOMROOM = "customRoom";

		public const string LEVELNAME = "levelName";

		public const string GAMEMODE = "gameMode";

		public const string PASSWORD = "password";

		public const string CLIENT_VERSION = "client_version";

		public const string PLATFORM = "platform";

		public const string GUID = "guid";
	}

	private PrivateMatchState _state;

	private PrivateMatchState _lastState;

	private GameObject _currentPopup;

	private GameObject _errorMessage;

	private float _timeJoinedRoom;

	[SerializeField]
	private GameObject _promptPopup;

	[SerializeField]
	private GameObject _searchPopup;

	[SerializeField]
	private GameObject _createPopup;

	[SerializeField]
	private GameObject _lobbyPopup;

	[SerializeField]
	private GameObject _errorMessagePrefab;

	private string _createRoomFailed = "Failed to create the room, please try again";

	protected override void Awake()
	{
		_createRoomFailed = Language.Get("CREATE_MATCH_ROOM_FAILED_STATUS");
		ChangeState(_state);
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.networkingPeer.ChannelCount = 9;
			string text = ServiceManager.Instance.GetPhotonAppID();
			if (string.IsNullOrEmpty(text))
			{
				text = "b2f1c620-3f72-4bc6-a80a-eeabcb8a948c";
			}
			PhotonNetwork.Connect("app-us.exitgamescloud.com", 5055, text, "s09tea268se1aad9751089asn1876297ep");
		}
	}

	public void ChangeState(PrivateMatchState state)
	{
		CloseCurrentPopup();
		_lastState = _state;
		Stats stats = ServiceManager.Instance.GetStats();
		switch (state)
		{
		case PrivateMatchState.PROMPT:
			EventTracker.TrackEvent(new PrivateMatchSelectOpenedSchema(new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level)));
			_currentPopup = UnityEngine.Object.Instantiate(_promptPopup, _promptPopup.transform.position, _promptPopup.transform.rotation) as GameObject;
			break;
		case PrivateMatchState.SEARCH:
			EventTracker.TrackEvent(new JoinPrivateMatchOpenedSchema(new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level)));
			_currentPopup = UnityEngine.Object.Instantiate(_searchPopup, _searchPopup.transform.position, _searchPopup.transform.rotation) as GameObject;
			break;
		case PrivateMatchState.CREATE:
			EventTracker.TrackEvent(new CreatePrivateMatchOpenedSchema(new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level)));
			_currentPopup = UnityEngine.Object.Instantiate(_createPopup, _createPopup.transform.position, _createPopup.transform.rotation) as GameObject;
			break;
		case PrivateMatchState.LOBBY:
			_currentPopup = UnityEngine.Object.Instantiate(_lobbyPopup, _lobbyPopup.transform.position, _lobbyPopup.transform.rotation) as GameObject;
			break;
		}
		if (_currentPopup != null)
		{
			_currentPopup.transform.parent = base.transform;
			_currentPopup.SendMessage("SetController", this, SendMessageOptions.DontRequireReceiver);
			if (_currentPopup.animation != null && _currentPopup.animation.GetClip("in") != null)
			{
				_currentPopup.animation.Play("in");
			}
		}
		_state = state;
	}

	public bool CreateRoom(string roomName, ExitGames.Client.Photon.Hashtable roomProperties)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomName == roomInfo.name)
			{
				return false;
			}
		}
		_timeJoinedRoom = Time.realtimeSinceStartup;
		if (roomProperties.ContainsKey("gameMode"))
		{
			Preferences.Instance.CurrentGameMode = (GameMode)(int)Enum.Parse(typeof(GameMode), (string)roomProperties["gameMode"]);
		}
		else
		{
			Preferences.Instance.CurrentGameMode = GameMode.TB;
		}
		PhotonNetwork.player.SetCustomProperties(UserParameters());
		roomProperties.Add("guid", Guid.NewGuid().ToString());
		PhotonNetwork.CreateRoom(roomName, true, true, 8, roomProperties, new string[7] { "levelName", "gameMode", "password", "customRoom", "client_version", "platform", "guid" });
		Stats stats = ServiceManager.Instance.GetStats();
		EventTracker.TrackEvent(new PrivateMatchCreatedSchema(new MatchTypeParameter((GameMode)(int)Enum.Parse(typeof(GameMode), roomProperties["gameMode"] as string)), new StageParameter(roomProperties["levelName"] as string), new PasswordProtectedParameter(!string.IsNullOrEmpty(roomProperties["password"] as string)), new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level)));
		return true;
	}

	public void JoinRoom(RoomInfo roomToJoin)
	{
		_timeJoinedRoom = Time.realtimeSinceStartup;
		if (roomToJoin.customProperties.ContainsKey("gameMode"))
		{
			Preferences.Instance.CurrentGameMode = (GameMode)(int)Enum.Parse(typeof(GameMode), (string)roomToJoin.customProperties["gameMode"]);
		}
		else
		{
			Preferences.Instance.CurrentGameMode = GameMode.TB;
		}
		PhotonNetwork.player.SetCustomProperties(UserParameters());
		PhotonNetwork.JoinRoom(roomToJoin.name);
		Stats stats = ServiceManager.Instance.GetStats();
		EventTracker.TrackEvent(new PrivateMatchJoinedSchema(new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level)));
	}

	public void StartGame()
	{
		ServiceManager.Instance.IsPrivateMatch = true;
		StartCoroutine(LoadLevel((string)PhotonNetwork.room.customProperties["levelName"]));
		PhotonNetwork.networkingPeer.OpRaiseEvent(106, null, true, 0);
		EventTracker.TrackEvent(new PrivateMatchStartedSchema(new PrivateMatchWaitTimeParameter(Mathf.RoundToInt(Time.realtimeSinceStartup - _timeJoinedRoom)), new UsersInMatchParameter(PhotonNetwork.playerList.Length)));
	}

	public void Close()
	{
		if (_state == PrivateMatchState.LOBBY)
		{
			EventTracker.TrackEvent(new PrivateMatchLeftSchema(new PrivateMatchWaitTimeParameter(Mathf.RoundToInt(Time.realtimeSinceStartup - _timeJoinedRoom))));
		}
		else
		{
			EventTracker.TrackEvent(new PrivateMatchSelectClosedSchema());
		}
		float t = CloseCurrentPopup();
		PhotonNetwork.Disconnect();
		PhotonNetwork.player.ClearBBRProperties();
		UnityEngine.Object.Destroy(base.gameObject, t);
	}

	public void ChangeToLastState()
	{
		ChangeState(_lastState);
	}

	public void DisplayError(string errorMessage)
	{
		_errorMessage = UnityEngine.Object.Instantiate(_errorMessagePrefab, _errorMessagePrefab.transform.position, _errorMessagePrefab.transform.rotation) as GameObject;
		_errorMessage.transform.parent = base.transform;
		_errorMessage.GetComponentInChildren<TextBlock>().OnSetText(errorMessage.ToUpper(), string.Empty);
	}

	private void CloseError()
	{
		if (_errorMessage.animation != null && (bool)_errorMessage.animation.GetClip("out"))
		{
			_errorMessage.animation.Play("out");
		}
		else
		{
			UnityEngine.Object.Destroy(_errorMessage);
		}
	}

	private float CloseCurrentPopup()
	{
		float result = 0f;
		if (_currentPopup != null)
		{
			if (_currentPopup.animation != null && _currentPopup.animation.GetClip("out") != null)
			{
				_currentPopup.animation.Play("out");
				result = _currentPopup.animation.GetClip("out").length;
			}
			else
			{
				UnityEngine.Object.Destroy(_currentPopup);
				result = 0f;
			}
		}
		return result;
	}

	private void OnGUIButtonClicked(GUIButton button)
	{
		switch (button.name)
		{
		case "prompt_join":
			ChangeState(PrivateMatchState.SEARCH);
			break;
		case "prompt_create":
			ChangeState(PrivateMatchState.CREATE);
			break;
		case "prompt_close":
			Close();
			break;
		case "error_close":
			CloseError();
			break;
		}
	}

	private IEnumerator LoadLevel(string levelName)
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.open = false;
		}
		PhotonNetwork.isMessageQueueRunning = true;
		CumulativeStats.Instance.OnSaveStats();
		string matchName = levelName + ":";
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer player in playerList)
		{
			matchName = matchName + (int)player.customProperties[(byte)225] + ":";
		}
		matchName = string.Concat(matchName, PhotonNetwork.room.name + "|" + PhotonNetwork.room.customProperties["guid"]);
		ServiceManager.Instance.SetMatchGameName(matchName);
		SoundManager.Instance.pauseMusic();
		MemorySweep.levelToLoad = levelName;
		Application.LoadLevel("MemorySweep");
		yield return null;
		PhotonNetwork.isMessageQueueRunning = false;
	}

	private ExitGames.Client.Photon.Hashtable UserParameters()
	{
		ExitGames.Client.Photon.Hashtable photonUserParametersForCurrentLoadout = LoadoutManager.Instance.GetPhotonUserParametersForCurrentLoadout();
		photonUserParametersForCurrentLoadout.Add((byte)225, ServiceManager.Instance.GetStats().pid);
		return photonUserParametersForCurrentLoadout;
	}

	private int BlueTeamCount()
	{
		int num = 0;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if (photonPlayer.customProperties.ContainsKey((byte)86) && (int)photonPlayer.customProperties[(byte)86] == 1)
			{
				num++;
			}
		}
		return num;
	}

	private int RedTeamCount()
	{
		int num = 0;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if (photonPlayer.customProperties.ContainsKey((byte)86) && (int)photonPlayer.customProperties[(byte)86] == 0)
			{
				num++;
			}
		}
		return num;
	}

	private void OnJoinedRoom()
	{
		ChangeState(PrivateMatchState.LOBBY);
		int num = RedTeamCount();
		int num2 = BlueTeamCount();
		if (Preferences.Instance.CurrentGameMode == GameMode.FFA)
		{
			if (num < 4)
			{
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					(byte)86,
					0
				} });
			}
			else
			{
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					(byte)86,
					1
				} });
			}
			return;
		}
		if (num >= 4)
		{
			if (num2 < 4)
			{
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					(byte)86,
					1
				} });
			}
			else
			{
				Close();
			}
		}
		if (num2 >= 4)
		{
			if (num < 4)
			{
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					(byte)86,
					0
				} });
			}
			else
			{
				Close();
			}
		}
		if (num > num2)
		{
			Debug.Log("Adding to blue because red > blue");
			PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
			{
				(byte)86,
				1
			} });
		}
		else
		{
			Debug.Log("Adding to red because blue > red");
			PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
			{
				(byte)86,
				0
			} });
		}
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log("Failed to create room!");
		DisplayError(_createRoomFailed);
		_currentPopup.SendMessage("ReEnableButtons");
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape) && _errorMessage != null)
		{
			CloseError();
		}
	}
}
