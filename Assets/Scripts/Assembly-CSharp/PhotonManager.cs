using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
	public class PhotonSkillUser
	{
		public int skill;

		public int id;

		public PhotonUser photonUser;

		public PhotonSkillUser(int s, int i, PhotonUser p)
		{
			skill = s;
			id = i;
			photonUser = p;
		}
	}

	public delegate void IngameUpdate();

	public delegate void NetworkTick(int serverTime);

	public delegate void HandleCustomEvent(byte evCode, ref ExitGames.Client.Photon.Hashtable data, int senderID);

	public delegate void HandleCustomPeerReturn(byte opCode, int returnCode, ref ExitGames.Client.Photon.Hashtable returnValues, short invocID);

	public delegate void HandleGetProperties(ExitGames.Client.Photon.Hashtable properties, short invocID);

	private static PhotonManager instance;

	private PhotonPeer peer;

	private PhotonState currentPhotonState;

	private string gameName = string.Empty;

	private ExitGames.Client.Photon.Hashtable localUserParameters;

	private Dictionary<int, PhotonUser> userList = new Dictionary<int, PhotonUser>();

	private int localUserID = -1;

	private Type userType = typeof(PhotonUser);

	private float networkTickInterval = 0.1f;

	private float lastNetworkTick;

	private int clientDelayMS;

	private int clientMode;

	private int numUsersToCreate;

	[SerializeField]
	private int maxRoomWaitSeconds = 30;

	private bool hasAssignedTeams;

	private int numServerObjectsToSync;

	[HideInInspector]
	public int numObjectsToCreate;

	public int MAX_PLAYERS = -1;

	public TextMesh lobbyWaitString;

	public TextMesh lobbyAutostartString;

	public List<PhotonSkillUser> playerSkillUsers = new List<PhotonSkillUser>();

	public GameObject persistentCodeFolder;

	private string _waitingForPlayer;

	private string _waitingForPlayers;

	private IngameUpdate ingameUpdate;

	private NetworkTick networkTick;

	public static PhotonManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (PhotonManager)UnityEngine.Object.FindObjectOfType(typeof(PhotonManager));
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	public PhotonPeer NPeer
	{
		get
		{
			return peer;
		}
	}

	public PhotonState CurrentState
	{
		get
		{
			return currentPhotonState;
		}
	}

	public float NetworkTickInterval
	{
		get
		{
			return networkTickInterval;
		}
	}

	public bool IsInGame
	{
		get
		{
			return currentPhotonState == PhotonState.Joined;
		}
	}

	public int LocalUserID
	{
		get
		{
			return localUserID;
		}
	}

	public PhotonUser LocalUser
	{
		get
		{
			return userList[localUserID];
		}
	}

	public Dictionary<int, PhotonUser> UserList
	{
		get
		{
			return userList;
		}
	}

	public int ClientMode
	{
		get
		{
			return clientMode;
		}
	}

	public int ClientDelayMS
	{
		get
		{
			return clientDelayMS;
		}
	}

	public int ServerTimeInMilliseconds
	{
		get
		{
			return PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
		}
	}

	public event EventHandler<OnConnectedEventArgs> OnConnected;

	public event EventHandler<OnJoinedEventArgs> OnJoined;

	public event EventHandler<OnStartPlayingEventArgs> OnStartPlaying;

	public event EventHandler<OnLeaveEventArgs> OnLeave;

	public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;

	public event EventHandler<OnExceptionEventArgs> OnException;

	public event EventHandler<OnUserCreatedEventArgs> OnUserCreated;

	public event EventHandler<OnUserDestroyedEventArgs> OnUserDestroyed;

	public event EventHandler<OnNetSyncObjectCreatedEventArgs> OnNetSyncObjectCreated;

	public void RegisterClient(IPhotonClient client, Type userType)
	{
		ingameUpdate = client.IngameUpdate;
		networkTick = client.NetworkTick;
		this.userType = userType;
	}

	private void RegisterNetSyncServerObjects()
	{
		object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(NetSyncServerObject));
		numServerObjectsToSync = array.Length;
		object[] array2 = array;
		foreach (object obj in array2)
		{
			NetSyncServerObject netSyncServerObject = (NetSyncServerObject)obj;
			NetSyncManager.Instance.RegisterPendingNetSyncServerObject(netSyncServerObject);
			RegisterNetSyncServerObject(netSyncServerObject.objectName);
		}
	}

	private void Awake()
	{
		UpdateLocalizationText();
		currentPhotonState = PhotonState.Disconnected;
		if (PhotonNetwork.connected)
		{
			currentPhotonState = PhotonState.Connected;
		}
		Application.runInBackground = true;
		PhotonNetwork.sendRate = 6;
		PhotonNetwork.sendRateOnSerialize = 6;
		NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;
		networkingPeer.OnPropertiesUpdated = (Action<PhotonPlayer>)Delegate.Combine(networkingPeer.OnPropertiesUpdated, new Action<PhotonPlayer>(OnPlayerPropertiesUpdated));
	}

	private void UpdateLocalizationText()
	{
		_waitingForPlayer = Language.Get("GAME_STATUS_WAITING_FOR_PLAYER");
		_waitingForPlayers = Language.Get("GAME_STATUS_WAITING_FOR_PLAYERS");
	}

	public void Connect(string appID, string url)
	{
		if (currentPhotonState == PhotonState.Disconnected)
		{
			currentPhotonState = PhotonState.Connecting;
			if (!PhotonNetwork.connected)
			{
				PhotonNetwork.networkingPeer.ChannelCount = 9;
				PhotonNetwork.Connect(url, 5055, appID, "s09tea268se1aad9751089asn1876297ep");
			}
			else if (!ServiceManager.Instance.IsPrivateMatch)
			{
				OnJoinedLobby();
			}
			PhotonNetwork.autoCleanUpPlayerObjects = false;
			PhotonNetwork.unreliableCommandsLimit = 30;
			PhotonNetwork.networkingPeer.DisconnectTimeout = 50000;
		}
	}

	public void Join(string roomName, ExitGames.Client.Photon.Hashtable userParameters)
	{
		if (currentPhotonState == PhotonState.Connected)
		{
			gameName = roomName;
			localUserParameters = userParameters;
			if (ServiceManager.Instance.IsPrivateMatch)
			{
				OnJoinedRoom();
				StartCoroutine(WaitForPlayers());
			}
			else
			{
				currentPhotonState = PhotonState.Joining;
			}
		}
	}

	public void Leave()
	{
		PhotonNetwork.player.ClearBBRProperties();
		NetSyncManager.Instance.CleanUp();
		PhotonNetwork.networkingPeer.SendOutgoingCommands();
		PhotonNetwork.networkingPeer.Service();
		PhotonNetwork.LeaveRoom();
		currentPhotonState = PhotonState.Leaving;
		gameName = string.Empty;
		clientMode = 0;
	}

	public void Disconnect()
	{
		NetSyncManager.Instance.CleanUp();
		PhotonNetwork.Disconnect();
		currentPhotonState = PhotonState.Disconnecting;
	}

	private void Update()
	{
		if (currentPhotonState == PhotonState.AwaitingUsers && hasAssignedTeams)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(109, true, new ExitGames.Client.Photon.Hashtable());
			currentPhotonState = PhotonState.AwaitingObjects;
			numUsersToCreate = 0;
		}
		if (currentPhotonState == PhotonState.AwaitingObjects && NetSyncManager.Instance.Objects.Count >= numObjectsToCreate)
		{
			currentPhotonState = PhotonState.Playing;
			numObjectsToCreate = 0;
			if (this.OnStartPlaying != null)
			{
				this.OnStartPlaying(this, new OnStartPlayingEventArgs());
			}
		}
		if (Time.time - lastNetworkTick > networkTickInterval)
		{
			if (networkTick != null && currentPhotonState == PhotonState.Playing)
			{
				networkTick(ServerTimeInMilliseconds);
			}
			NetSyncManager.Instance.OnNetTick(ServerTimeInMilliseconds);
			lastNetworkTick = Time.time;
		}
		if (currentPhotonState == PhotonState.Playing && ingameUpdate != null)
		{
			ingameUpdate();
		}
	}

	private void OnApplicationQuit()
	{
		Disconnect();
		PhotonNetwork.networkingPeer.Service();
		PhotonNetwork.networkingPeer.StopThread();
	}

	public void EventAction(byte eventCode, ExitGames.Client.Photon.Hashtable eventParams)
	{
	}

	public void OperationResult(byte opCode, int returnCode, ExitGames.Client.Photon.Hashtable returnValues, short invocID)
	{
	}

	public void PeerStatusCallback(StatusCode returnCode)
	{
	}

	public void DebugReturn(DebugLevel debugLevel, string debug)
	{
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
	}

	public void OnEvent(EventData eventData)
	{
	}

	private void CreateLocalUser()
	{
		PhotonUser photonUser = (PhotonUser)base.gameObject.AddComponent(userType);
		photonUser.Construct(localUserID, PhotonNetwork.player.customProperties, true);
		userList.Add(localUserID, photonUser);
		int num = MAX_PLAYERS - userList.Count;
		if (lobbyWaitString != null)
		{
			if (num > 1)
			{
				lobbyWaitString.text = string.Format(_waitingForPlayers, num);
			}
			else
			{
				lobbyWaitString.text = string.Format(_waitingForPlayer, num);
			}
		}
		if (!ServiceManager.Instance.IsPrivateMatch)
		{
			StartCoroutine(delayedAssignTeams());
		}
		if (MAX_PLAYERS == 1)
		{
			AssignTeams();
		}
	}

	public void CreateRemoteUser(ExitGames.Client.Photon.Hashtable data)
	{
		int num = 0;
		if (data.ContainsKey((byte)225))
		{
			num = (int)data[(byte)225];
			if (num == localUserID)
			{
				Debug.LogWarning("Trying to create local player from remote call");
				return;
			}
			if (userList.ContainsKey(num))
			{
				Debug.LogWarning("New remote user with same ActorNr already created, actorNr: " + num);
				return;
			}
			PhotonUser photonUser = (PhotonUser)base.gameObject.AddComponent(userType);
			photonUser.Construct(num, data, false);
			userList.Add(num, photonUser);
			int num2 = MAX_PLAYERS - userList.Count;
			if (lobbyWaitString != null)
			{
				if (num2 > 1)
				{
					lobbyWaitString.text = string.Format(_waitingForPlayers, num2);
				}
				else
				{
					lobbyWaitString.text = string.Format(_waitingForPlayer, num2);
				}
			}
			if (userList.Count == MAX_PLAYERS && !ServiceManager.Instance.IsPrivateMatch)
			{
				AssignTeams();
			}
		}
		else
		{
			Debug.LogError("actor number not found!");
		}
	}

	public IEnumerator delayedAssignTeams()
	{
		int autoTimeLeft = 20;
		while (autoTimeLeft > 0 && !hasAssignedTeams && lobbyAutostartString != null)
		{
			if (autoTimeLeft > 1)
			{
				lobbyAutostartString.text = "Game will autostart in " + autoTimeLeft + " seconds";
			}
			else
			{
				lobbyAutostartString.text = "Game will autostart in " + autoTimeLeft + " second";
			}
			yield return new WaitForSeconds(1f);
			autoTimeLeft--;
		}
		AssignTeams();
	}

	public void AssignTeams()
	{
		if (hasAssignedTeams)
		{
			return;
		}
		if (PhotonNetwork.isMasterClient)
		{
			if (PhotonNetwork.room != null)
			{
				PhotonNetwork.room.open = false;
				PhotonNetwork.room.visible = false;
			}
			ValidatePlayers();
		}
		hasAssignedTeams = true;
		if (lobbyWaitString != null)
		{
			lobbyWaitString.text = "Launching game...";
		}
		if (userList.Count < 2 && MAX_PLAYERS > 1)
		{
			Debug.Log("only have one player, launching single player since userlist count is: " + userList.Count + " and max players is: " + MAX_PLAYERS);
			currentPhotonState = PhotonState.Disconnecting;
			PhotonNetwork.Disconnect();
			MainMenu.gameCancelled = true;
			PhotonNetwork.player.ClearBBRProperties();
			Application.LoadLevel("MainMenu");
			return;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		MAX_PLAYERS = userList.Count;
		bool val = true;
		ServiceManager.Instance.UpdateProperty("use_skill_sorting", ref val);
		if (!val)
		{
			List<int> list = new List<int>(userList.Keys);
			list.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				userList[list[i]].UserParameters[(byte)86] = ((i % 2 != 0) ? 1 : 0);
			}
			for (int j = 0; j < list.Count; j++)
			{
				userList[list[j]].OnCreateObject();
				if (this.OnUserCreated != null)
				{
					this.OnUserCreated(this, new OnUserCreatedEventArgs(userList[list[j]].UserID, userList[list[j]].UserParameters, userList[list[j]].isLocal));
				}
			}
		}
		else
		{
			playerSkillUsers = new List<PhotonSkillUser>();
			foreach (KeyValuePair<int, PhotonUser> user in userList)
			{
				int i2 = (int)user.Value.UserParameters[(byte)106];
				int s = (int)user.Value.UserParameters[(byte)108];
				playerSkillUsers.Add(new PhotonSkillUser(s, i2, user.Value));
			}
			playerSkillUsers.Sort(delegate(PhotonSkillUser a, PhotonSkillUser b)
			{
				if (a.skill < b.skill)
				{
					return 1;
				}
				if (a.skill > b.skill)
				{
					return -1;
				}
				if (a.skill == b.skill)
				{
					if (a.id > b.id)
					{
						return 1;
					}
					if (a.id < b.id)
					{
						return -1;
					}
				}
				return 0;
			});
			if (!ServiceManager.Instance.IsPrivateMatch)
			{
				for (int k = 0; k < playerSkillUsers.Count; k++)
				{
					playerSkillUsers[k].photonUser.UserParameters[(byte)86] = ((k % 2 != 0) ? 1 : 0);
				}
			}
			for (int l = 0; l < playerSkillUsers.Count; l++)
			{
				playerSkillUsers[l].photonUser.OnCreateObject();
				if (this.OnUserCreated != null)
				{
					this.OnUserCreated(this, new OnUserCreatedEventArgs(playerSkillUsers[l].photonUser.UserID, playerSkillUsers[l].photonUser.UserParameters, playerSkillUsers[l].photonUser.isLocal));
				}
			}
		}
		PhotonNetwork.isMessageQueueRunning = true;
		ServiceManager.Instance.NotifyGameStart(null, FailedStartGame);
	}

	public void ValidatePlayers()
	{
		bool flag = false;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if (!photonPlayer.customProperties.ContainsKey((byte)225) && !ValidatePlayer(photonPlayer.customProperties))
			{
				if (photonPlayer == PhotonNetwork.player)
				{
					flag = true;
					continue;
				}
				Debug.LogError("Kicking player bc they are invalid!");
				PhotonNetwork.CloseConnection(photonPlayer);
			}
			else if (ServiceManager.Instance.IsPrivateMatch && !ValidatePlayer(photonPlayer.customProperties))
			{
				PhotonNetwork.CloseConnection(photonPlayer);
			}
		}
		if (flag)
		{
			StartCoroutine(DelayedMasterLeave());
		}
	}

	private bool ValidatePlayer(ExitGames.Client.Photon.Hashtable userParams)
	{
		return userParams.ContainsKey((byte)86) && userParams.ContainsKey((byte)90) && userParams.ContainsKey((byte)89) && userParams.ContainsKey((byte)91) && userParams.ContainsKey((byte)87) && userParams.ContainsKey((byte)88);
	}

	private IEnumerator DelayedMasterLeave()
	{
		yield return new WaitForSeconds(1f);
		PhotonNetwork.LeaveRoom();
	}

	public void RegisterNetSyncObject(string objectType, Vector3 position, float baseRotY, float angleRotX, int state)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)73, position.x);
		hashtable.Add((byte)74, position.y);
		hashtable.Add((byte)75, position.z);
		hashtable.Add((byte)76, baseRotY);
		hashtable.Add((byte)77, angleRotX);
		hashtable.Add((byte)80, state);
		hashtable.Add((byte)81, objectType);
		hashtable.Add((byte)82, localUserID);
		hashtable.Add((byte)72, NetSyncManager.Instance.CurrentNetSyncID);
		PhotonNetwork.networkingPeer.OpRaiseEvent(93, hashtable, true, 0);
		CreateNetSyncObject((long)hashtable[(byte)72], objectType, state, localUserID, position.x, position.y, position.z, baseRotY, angleRotX);
	}

	public void RegisterNetSyncServerObject(string name)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)97, name);
		hashtable.Add((byte)72, NetSyncManager.Instance.CurrentNetSyncID);
		PhotonNetwork.networkingPeer.OpRaiseEvent(123, hashtable, true, 0);
	}

	public void UnregisterNetSyncObject(long netID)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)72, netID);
		PhotonNetwork.networkingPeer.OpRaiseEvent(94, hashtable, true, 0);
	}

	public void CreateNetSyncObject(long netID, string objectType, int state, int ownerUserID, float posX, float posY, float posZ, float baseRotY, float angleRotX)
	{
		if (!userList.ContainsKey(ownerUserID))
		{
			return;
		}
		Vector3 startPos = new Vector3(posX, posY, posZ);
		if (!userList[ownerUserID].UserParameters.ContainsKey((byte)86))
		{
			if (hasAssignedTeams)
			{
				Debug.LogError("Tried to create user, but they didnt have a team after teams were assigned");
				return;
			}
			if (!ServiceManager.Instance.IsPrivateMatch)
			{
				AssignTeams();
			}
		}
		NetSyncObject netSyncObject = (NetSyncObject)UserList[ownerUserID].gameObject.AddComponent(objectType);
		netSyncObject.Construct(netID, startPos, baseRotY, angleRotX, state, ownerUserID);
		if (this.OnNetSyncObjectCreated != null)
		{
			this.OnNetSyncObjectCreated(this, new OnNetSyncObjectCreatedEventArgs(ownerUserID, netID));
		}
	}

	public void ReportTransformUpdate(long netID, GameObject gameObject, Transform angleTransform)
	{
		if (!(gameObject == null))
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			Transform transform = gameObject.transform;
			hashtable.Add((byte)72, netID);
			hashtable.Add((byte)83, ServerTimeInMilliseconds);
			hashtable.Add((byte)73, transform.position.x);
			hashtable.Add((byte)74, transform.position.y);
			hashtable.Add((byte)75, transform.position.z);
			hashtable.Add((byte)76, transform.eulerAngles.y);
			hashtable.Add((byte)77, angleTransform.localEulerAngles.x);
			OpRaiseEvent(92, hashtable, false, (byte)PhotonNetwork.player.ID);
		}
	}

	public void ReportStateUpdate(long netID, int state)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)72, netID);
		hashtable.Add((byte)80, (byte)state);
		hashtable.Add((byte)83, ServerTimeInMilliseconds);
		OpRaiseEvent(97, hashtable, true, (byte)PhotonNetwork.player.ID);
	}

	public void ReportFireProjectileUpdate(long netID, Vector3 pos, Vector3 velocity)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)72, netID);
		hashtable.Add((byte)98, pos.x);
		hashtable.Add((byte)99, pos.y);
		hashtable.Add((byte)100, pos.z);
		hashtable.Add((byte)101, velocity.x);
		hashtable.Add((byte)102, velocity.y);
		hashtable.Add((byte)103, velocity.z);
		hashtable.Add((byte)83, ServerTimeInMilliseconds);
		OpRaiseEvent(105, hashtable, true, (byte)PhotonNetwork.player.ID);
	}

	public void ReportActionUpdate(long netID, byte action, ExitGames.Client.Photon.Hashtable parameters)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)72, netID);
		hashtable.Add((byte)95, action);
		hashtable.Add((byte)96, parameters);
		hashtable.Add((byte)83, ServerTimeInMilliseconds);
		OpRaiseEvent(104, hashtable, true, (byte)PhotonNetwork.player.ID);
	}

	private void OpRaiseEvent(byte eventCode, ExitGames.Client.Photon.Hashtable parameters, bool sendReliable, byte channel)
	{
		if (channel < PhotonNetwork.networkingPeer.ChannelCount && channel >= 0)
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, parameters, sendReliable, channel);
		}
		else
		{
			PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, parameters, sendReliable, 0);
		}
	}

	public void OnJoinedLobby()
	{
		currentPhotonState = PhotonState.Connected;
		string[] array = ServiceManager.Instance.GetMatchGameName().Split(':');
		if (ServiceManager.Instance.GetStats().pid.ToString() == array[1])
		{
			PhotonNetwork.CreateRoom(ServiceManager.Instance.GetMatchGameName(), true, true, Mathf.Max(8, array.Length));
		}
		else
		{
			StartCoroutine(LookForGameRoom());
		}
	}

	public void OnJoinedRoom()
	{
		currentPhotonState = PhotonState.AwaitingUsers;
		localUserID = ServiceManager.Instance.GetStats().pid;
		if (!ServiceManager.Instance.IsPrivateMatch)
		{
			localUserParameters.Add((byte)225, localUserID);
			PhotonNetwork.player.SetCustomProperties(localUserParameters);
		}
		else
		{
			localUserID = (int)PhotonNetwork.player.customProperties[(byte)106];
		}
		string[] array = ServiceManager.Instance.GetMatchGameName().Split(':');
		for (int i = 1; i < array.Length - 1; i++)
		{
			if (ServiceManager.Instance.GetStats().pid.ToString() == array[i])
			{
				NetSyncManager.Instance.CurrentNetSyncID = PhotonNetwork.player.ID * 10000000;
			}
		}
		CreateLocalUser();
		for (int j = 0; j < PhotonNetwork.otherPlayers.Length; j++)
		{
			if (PhotonNetwork.otherPlayers[j].ID > -1)
			{
				if (PhotonNetwork.otherPlayers[j].customProperties != null && PhotonNetwork.otherPlayers[j].customProperties.Count > 0)
				{
					CreateRemoteUser(PhotonNetwork.otherPlayers[j].customProperties);
				}
				else
				{
					Debug.LogError("OnJoinedRoom player properties is null");
				}
			}
		}
	}

	public void OnLeftRoom()
	{
		currentPhotonState = PhotonState.Connected;
		if (this.OnLeave != null)
		{
			this.OnLeave(this, new OnLeaveEventArgs(localUserID));
		}
		if (this.OnUserDestroyed != null)
		{
			this.OnUserDestroyed(this, new OnUserDestroyedEventArgs(localUserID, true));
		}
	}

	public void OnDisconnectedFromPhoton()
	{
		EventTracker.TrackEvent(MatchEventsHelper.MatchExited(true));
		currentPhotonState = PhotonState.Disconnected;
		if (this.OnLeave != null)
		{
			this.OnLeave(this, new OnLeaveEventArgs(localUserID));
		}
		if (this.OnUserDestroyed != null)
		{
			this.OnUserDestroyed(this, new OnUserDestroyedEventArgs(localUserID, true));
		}
		foreach (int key in userList.Keys)
		{
			userList[key].Destroy();
		}
		userList.Clear();
		localUserID = -1;
		ServiceManager.Instance.LogGameLeft("user_quit");
		MainMenu.wasDisconnected = true;
		PhotonNetwork.player.ClearBBRProperties();
		Application.LoadLevel("MainMenu");
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		int iD = otherPlayer.ID;
		if (userList.ContainsKey(iD))
		{
			if (userList[iD] != null)
			{
				userList[iD].Destroy();
			}
			userList.Remove(iD);
		}
		Dictionary<long, NetSyncObject> dictionary = new Dictionary<long, NetSyncObject>(5);
		if (otherPlayer.customProperties.ContainsKey((byte)225))
		{
			foreach (KeyValuePair<long, NetSyncObject> @object in NetSyncManager.Instance.Objects)
			{
				if (@object.Value != null && @object.Value.OwnerID == (int)otherPlayer.customProperties[(byte)225])
				{
					dictionary.Add(@object.Key, @object.Value);
				}
			}
		}
		foreach (KeyValuePair<long, NetSyncObject> item in dictionary)
		{
			if (item.Value != null)
			{
				NetSyncManager.Instance.UnregisterNetSyncObject(item.Value.NetID);
			}
		}
		dictionary.Clear();
		dictionary = null;
	}

	public void OnPlayerPropertiesUpdated(PhotonPlayer player)
	{
		if (player.customProperties.Count > 0 && !hasAssignedTeams && !ServiceManager.Instance.IsPrivateMatch)
		{
			CreateRemoteUser(player.customProperties);
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient && hasAssignedTeams)
		{
			Debug.LogError("Kicking player because we already assigned teams!");
			PhotonNetwork.CloseConnection(player);
		}
	}

	public void OnCreatedRoom()
	{
		PhotonNetwork.room.maxPlayers = ServiceManager.Instance.GetMatchGameName().Split(':').Length - 1;
	}

	public void OnPhotonJoinRoomFailed()
	{
		Debug.LogError("PlayerClient failed to join room!");
		FailedToJoinGame();
	}

	public void OnPhotonCreateRoomFailed()
	{
		Debug.LogError("PlayerClient CreateRoom Failed!");
		FailedToJoinGame();
	}

	private IEnumerator LookForGameRoom(Action onSuccess = null, Action onFailure = null)
	{
		if (PhotonNetwork.room != null)
		{
			Debug.Log("already in a room, from private match");
			if (onSuccess != null)
			{
				onSuccess();
			}
			yield break;
		}
		bool foundRoom = false;
		int timeOutCount = 20;
		while (!foundRoom && timeOutCount >= 0)
		{
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			foreach (RoomInfo room in roomList)
			{
				if (room.name == ServiceManager.Instance.GetMatchGameName())
				{
					foundRoom = true;
				}
			}
			timeOutCount--;
			yield return new WaitForSeconds(1f);
		}
		if ((float)timeOutCount < 0f && !foundRoom)
		{
			Debug.Log(">>>>>>>>> Can't find the specified room");
			Instance.FailedToJoinGame();
			if (onFailure != null)
			{
				onFailure();
			}
		}
		else
		{
			Debug.Log(">>>>>>>>> found the specified room");
			if (onSuccess != null)
			{
				onSuccess();
			}
			PhotonNetwork.JoinRoom(ServiceManager.Instance.GetMatchGameName());
		}
	}

	public void FailedToJoinGame()
	{
		currentPhotonState = PhotonState.Disconnecting;
		PhotonNetwork.Disconnect();
		MainMenu.wasDisconnected = true;
		PhotonNetwork.player.ClearBBRProperties();
		Application.LoadLevel("MainMenu");
	}

	private IEnumerator WaitForPlayers()
	{
		PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "ready", true } });
		if (PhotonNetwork.isMasterClient)
		{
			ValidatePlayers();
			yield return null;
			for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
			{
				if (!ValidatePlayer(PhotonNetwork.playerList[i].customProperties))
				{
					Debug.LogError("Kicking player on second check because invalid");
					PhotonNetwork.CloseConnection(PhotonNetwork.playerList[i]);
				}
			}
			yield return null;
		}
		int readyCount = 0;
		float timeOut = 21f;
		while (readyCount < PhotonNetwork.playerList.Length && timeOut > 0f)
		{
			readyCount = 0;
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer player in playerList)
			{
				if (player.customProperties.ContainsKey("ready"))
				{
					readyCount++;
				}
			}
			if (lobbyWaitString != null)
			{
				lobbyWaitString.text = string.Format(_waitingForPlayers, (PhotonNetwork.playerList.Length - readyCount).ToString());
			}
			yield return new WaitForSeconds(0.5f);
			timeOut -= 0.5f;
		}
		AssignTeams();
	}

	private void FailedStartGame()
	{
		if (ServiceManager.Instance.LastError == "Invalid session")
		{
			currentPhotonState = PhotonState.Disconnected;
			PhotonNetwork.Disconnect();
			MainMenu.hasInvalidSession = true;
			PhotonNetwork.player.ClearBBRProperties();
			Application.LoadLevel("MainMenu");
		}
	}

	private void OnDestroy()
	{
		NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;
		networkingPeer.OnPropertiesUpdated = (Action<PhotonPlayer>)Delegate.Remove(networkingPeer.OnPropertiesUpdated, new Action<PhotonPlayer>(OnPlayerPropertiesUpdated));
	}
}
