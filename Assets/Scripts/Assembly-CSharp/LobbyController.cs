using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Utils.Coroutines;

public class LobbyController : MonoBehaviour
{
	private PrivateMatchController _controller;

	private List<PlayerPlate> _redPlayerPlates;

	private List<PlayerPlate> _bluePlayerPlates;

	private GameObject _currentBootPlayerPopup;

	private int _playerIDToKick = -1;

	private bool _leftButtonPressed;

	private bool _playButtonPressed;

	private float _roomAnnouncementCooldown = 15f;

	[SerializeField]
	private LevelSelect _levels;

	[SerializeField]
	private ModeSelect _modes;

	[SerializeField]
	private GameObject[] _masterOnlyObjects;

	[SerializeField]
	private GameObject _playBtn;

	[SerializeField]
	private GameObject _leaveBtn;

	[SerializeField]
	private GameObject _playerPlatePrefab;

	[SerializeField]
	private float _redStartPoint;

	[SerializeField]
	private float _spaceBetweenPlayerPlates;

	[SerializeField]
	private GameObject _bootPlayerPopup;

	[SerializeField]
	private TextBlock _roomName;

	[SerializeField]
	private GameObject _announcementCanvas;

	[SerializeField]
	private GameObject _passwordOptionContainer;

	[SerializeField]
	private Button _sendRoomAnnouncementButton;

	[SerializeField]
	private Toggle _sendPasswordToggle;

	private string _kickText = "You were booted from the room by the owner";

	private float BlueStartPoint
	{
		get
		{
			return _redStartPoint - _spaceBetweenPlayerPlates * 4f;
		}
	}

	private static event Action<string> _roomAnnouncementRequested;

	public static event Action<string> RoomAnnouncementRequested
	{
		add
		{
			LobbyController._roomAnnouncementRequested = (Action<string>)Delegate.Combine(LobbyController._roomAnnouncementRequested, value);
		}
		remove
		{
			LobbyController._roomAnnouncementRequested = (Action<string>)Delegate.Remove(LobbyController._roomAnnouncementRequested, value);
		}
	}

	private void Start()
	{
		UpdateLocalizedText();
		ServiceManager.Instance.UpdateProperty("room_announcement_cooldown", ref _roomAnnouncementCooldown);
		_redPlayerPlates = new List<PlayerPlate>();
		_bluePlayerPlates = new List<PlayerPlate>();
		NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;
		networkingPeer.OnPropertiesUpdated = (Action<PhotonPlayer>)Delegate.Combine(networkingPeer.OnPropertiesUpdated, new Action<PhotonPlayer>(OnPlayerPropertiesUpdated));
		if (PhotonNetwork.room != null)
		{
			ConstructMenu();
		}
	}

	private void UpdateLocalizedText()
	{
		_kickText = Language.Get("MATCH_LOBBY_KICKED_PLAYER_STATUS");
	}

	private void SetController(PrivateMatchController controller)
	{
		_controller = controller;
	}

	private void OnGUIButtonClicked(GUIButton button)
	{
		switch (button.name)
		{
		case "play":
			_playButtonPressed = true;
			button.disable();
			_controller.StartGame();
			break;
		case "leave":
			button.disable();
			_leftButtonPressed = true;
			PhotonNetwork.LeaveRoom();
			break;
		case "ok":
			CloseConnection(_playerIDToKick);
			CloseBootPopup();
			break;
		case "cancel":
			CloseBootPopup();
			break;
		default:
			Debug.LogError("no case for: " + button.name + " found.");
			break;
		}
	}

	private void ConstructMenu()
	{
		_roomName.OnSetText(PhotonNetwork.room.name, string.Empty);
		for (int i = 0; i < _masterOnlyObjects.Length; i++)
		{
			_masterOnlyObjects[i].SetActive(PhotonNetwork.isMasterClient);
		}
		if (PhotonNetwork.isMasterClient)
		{
			_playBtn.SetActive(true);
			_leaveBtn.SetActive(false);
			_announcementCanvas.SetActive(true);
			string password = PhotonNetwork.room.customProperties["password"] as string;
			_passwordOptionContainer.SetActive(!string.IsNullOrEmpty(password));
			_sendRoomAnnouncementButton.onClick.AddListener(delegate
			{
				if (LobbyController._roomAnnouncementRequested != null)
				{
					string text = "Join my room: " + PhotonNetwork.room.name;
					if (_sendPasswordToggle.isOn)
					{
						text = text + ", pw: " + password;
					}
					LobbyController._roomAnnouncementRequested(text);
					_sendRoomAnnouncementButton.interactable = false;
					StartCoroutine(new RunAfterRealtimeSeconds(_roomAnnouncementCooldown, delegate
					{
						_sendRoomAnnouncementButton.interactable = true;
					}));
				}
			});
		}
		else
		{
			_leaveBtn.SetActive(true);
			_playBtn.SetActive(false);
			_announcementCanvas.SetActive(false);
		}
		_levels.SetLevel((string)PhotonNetwork.room.customProperties["levelName"]);
		_modes.SetMode((GameMode)(int)Enum.Parse(typeof(GameMode), (string)PhotonNetwork.room.customProperties["gameMode"]));
		if (PhotonNetwork.room.playerCount <= 1)
		{
			_playBtn.GetComponent<GUIButton>().disable();
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			ConstructPlayerPlate(photonPlayer.customProperties, IsPlayerLocal(photonPlayer));
		}
	}

	private void ConstructPlayerPlate(Hashtable userParams, bool isLocal)
	{
		PlayerParameterModel playerParameterModel = new PlayerParameterModel(userParams);
		int playerTeam = playerParameterModel.PlayerTeam;
		GameObject gameObject = UnityEngine.Object.Instantiate(_playerPlatePrefab) as GameObject;
		gameObject.transform.parent = base.transform;
		PlayerPlate component = gameObject.GetComponent<PlayerPlate>();
		if (component != null)
		{
			if (isLocal)
			{
				component.SetKickToGrey();
			}
			component.ConstructPlayerPlate(playerParameterModel, PhotonNetwork.isMasterClient);
		}
		switch (playerTeam)
		{
		case 1:
			gameObject.transform.localPosition = new Vector3(0f, BlueStartPoint - (float)_bluePlayerPlates.Count * _spaceBetweenPlayerPlates, 1f);
			if (component != null)
			{
				_bluePlayerPlates.Add(component);
			}
			else
			{
				Debug.LogError("Player plate is null!");
			}
			break;
		case 0:
			gameObject.transform.localPosition = new Vector3(0f, _redStartPoint - (float)_redPlayerPlates.Count * _spaceBetweenPlayerPlates, 1f);
			if (component != null)
			{
				_redPlayerPlates.Add(component);
			}
			else
			{
				Debug.LogError("Player plate is null!");
			}
			break;
		}
	}

	private void BootPlayer(Hashtable parameters)
	{
		string text = (string)parameters[0];
		_playerIDToKick = (int)parameters[1];
		_currentBootPlayerPopup = UnityEngine.Object.Instantiate(_bootPlayerPopup, _bootPlayerPopup.transform.position, _bootPlayerPopup.transform.rotation) as GameObject;
		_currentBootPlayerPopup.transform.parent = base.transform;
		_currentBootPlayerPopup.name = text;
		if (_currentBootPlayerPopup.animation != null && _currentBootPlayerPopup.animation.GetClip("in") != null)
		{
			_currentBootPlayerPopup.animation.Play("in");
		}
	}

	private void CloseBootPopup()
	{
		if (_currentBootPlayerPopup != null && _currentBootPlayerPopup.animation != null && _currentBootPlayerPopup.animation.GetClip("out") != null)
		{
			_currentBootPlayerPopup.animation.Play("out");
		}
		else
		{
			UnityEngine.Object.Destroy(_currentBootPlayerPopup);
		}
	}

	private void CloseConnection(int playerID)
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if ((int)photonPlayer.customProperties[(byte)225] == playerID)
			{
				PhotonNetwork.CloseConnection(photonPlayer);
				break;
			}
		}
	}

	private bool IsPlayerLocal(PhotonPlayer player)
	{
		return player == PhotonNetwork.player;
	}

	private void OnJoinedRoom()
	{
		ConstructMenu();
	}

	private void OnPlayerPropertiesUpdated(PhotonPlayer player)
	{
		if (_playButtonPressed)
		{
			return;
		}
		if (PhotonNetwork.room.playerCount >= 2 && !_leftButtonPressed)
		{
			_playBtn.GetComponent<GUIButton>().enable();
		}
		int num = (int)player.customProperties[(byte)106];
		foreach (PlayerPlate redPlayerPlate in _redPlayerPlates)
		{
			if (num == redPlayerPlate.PlayerID)
			{
				Debug.LogWarning("we already have this player " + num);
				return;
			}
		}
		foreach (PlayerPlate bluePlayerPlate in _bluePlayerPlates)
		{
			if (num == bluePlayerPlate.PlayerID)
			{
				Debug.LogWarning("we already have this player " + num);
				return;
			}
		}
		ConstructPlayerPlate(player.customProperties, IsPlayerLocal(player));
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (PhotonNetwork.room.playerCount <= 1)
		{
			_playBtn.GetComponent<GUIButton>().disable();
		}
		int num = -1;
		int num2 = -1;
		PlayerPlate playerPlate = null;
		num = (int)player.customProperties[(byte)86];
		num2 = (int)player.customProperties[(byte)106];
		switch (num)
		{
		case 0:
			foreach (PlayerPlate redPlayerPlate in _redPlayerPlates)
			{
				if (redPlayerPlate.PlayerID == num2)
				{
					playerPlate = redPlayerPlate;
				}
			}
			break;
		case 1:
			foreach (PlayerPlate bluePlayerPlate in _bluePlayerPlates)
			{
				if (bluePlayerPlate.PlayerID == num2)
				{
					playerPlate = bluePlayerPlate;
				}
			}
			break;
		}
		if (playerPlate == null)
		{
			Debug.LogWarning("Could not find playerPlate");
			return;
		}
		switch (num)
		{
		case 0:
			_redPlayerPlates.Remove(playerPlate);
			UnityEngine.Object.Destroy(playerPlate.gameObject);
			break;
		case 1:
			_bluePlayerPlates.Remove(playerPlate);
			UnityEngine.Object.Destroy(playerPlate.gameObject);
			break;
		}
		ResetPlayerPlates();
	}

	private void OnMasterClientSwitched(PhotonPlayer newMasterPlayer)
	{
		if (PhotonNetwork.player != newMasterPlayer)
		{
			return;
		}
		foreach (PlayerPlate redPlayerPlate in _redPlayerPlates)
		{
			UnityEngine.Object.Destroy(redPlayerPlate.gameObject);
		}
		foreach (PlayerPlate bluePlayerPlate in _bluePlayerPlates)
		{
			UnityEngine.Object.Destroy(bluePlayerPlate.gameObject);
		}
		_redPlayerPlates.Clear();
		_bluePlayerPlates.Clear();
		ConstructMenu();
	}

	private void ResetPlayerPlates()
	{
		foreach (PlayerPlate redPlayerPlate in _redPlayerPlates)
		{
			UnityEngine.Object.Destroy(redPlayerPlate.gameObject);
		}
		foreach (PlayerPlate bluePlayerPlate in _bluePlayerPlates)
		{
			UnityEngine.Object.Destroy(bluePlayerPlate.gameObject);
		}
		_redPlayerPlates.Clear();
		_bluePlayerPlates.Clear();
		ConstructMenu();
	}

	private void OnLeftRoom()
	{
		if (!_leftButtonPressed)
		{
			_controller.DisplayError(_kickText);
			_controller.ChangeToLastState();
		}
		else
		{
			_controller.Close();
		}
	}

	private void OnDestroy()
	{
		NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;
		networkingPeer.OnPropertiesUpdated = (Action<PhotonPlayer>)Delegate.Remove(networkingPeer.OnPropertiesUpdated, new Action<PhotonPlayer>(OnPlayerPropertiesUpdated));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_leaveBtn.GetComponent<GUIButton>().disable();
			_leftButtonPressed = true;
			PhotonNetwork.LeaveRoom();
		}
	}
}
