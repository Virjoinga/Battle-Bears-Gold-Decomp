using System;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.JsonEncoders;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	public string ServerAddress;

	public string LocalAddress;

	public bool useLocalAddress;

	private PlayerManager PLAYER;

	private RoomManager ROOM;

	private SocketManager manager;

	private void Awake()
	{
		PLAYER = GetComponent<PlayerManager>();
		ROOM = GetComponent<RoomManager>();
	}

	private void Start()
	{
		Connect();
	}

	public void Connect()
	{
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.ReconnectionAttempts = 2;
		socketOptions.Reconnection = true;
		socketOptions.Timeout = TimeSpan.FromMilliseconds(5000.0);
		socketOptions.AutoConnect = false;
		if (useLocalAddress)
		{
			manager = new SocketManager(new Uri(LocalAddress), socketOptions);
		}
		else
		{
			manager = new SocketManager(new Uri(ServerAddress), socketOptions);
		}
		manager.Encoder = new LitJsonEncoder();
		JsonMapper.RegisterImporter((double input) => (int)(input + 0.5));
		manager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
		manager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
		manager.Socket.On(SocketIOEventTypes.Error, OnError);
		manager.Socket.On("reconnect", OnReconnect);
		manager.Socket.On("reconnecting", OnReconnecting);
		manager.Socket.On("reconnect_attempt", OnReconnectAttempt);
		manager.Socket.On("reconnect_failed", OnReconnectFailed);
		manager.Socket.On("Login", OnLogin);
		manager.Socket.On("JoinRoom", OnJoinRoom);
		manager.Socket.On("A", OnAddPlayer);
		manager.Socket.On("R", OnRemovePlayer);
		manager.Socket.On("M", OnMessage);
		manager.Socket.On("P", OnUpdatePosition);
		manager.Open();
	}

	private static T ConvertTo<T>(object obj)
	{
		JsonWriter jsonWriter = new JsonWriter();
		JsonMapper.ToJson(obj, jsonWriter);
		return JsonMapper.ToObject<T>(new JsonReader(jsonWriter.ToString()));
	}

	public void LoginSession(string ticket)
	{
		Debug.Log("LoginSession: " + ticket);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("SessionTicket", ticket);
		manager.Socket.Emit("LoginSession", dictionary);
	}

	private void OnLogin(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("Login: " + packet);
		PlayerData playerData = ConvertTo<PlayerData>(args[0]);
		PLAYER.PlayerId = playerData.PlayerId;
		PLAYER.Nickname = playerData.Nickname;
		PLAYER.CurrentRoom = playerData.CurrentRoom;
	}

	public void JoinRoom(string roomId)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("RoomId", roomId);
		manager.Socket.Emit("JoinRoom", dictionary);
	}

	private void OnJoinRoom(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("JoinRoom: " + packet);
		RoomData roomData = ConvertTo<RoomData>(args[0]);
		ROOM.JoinRoom("town");
		CrumbData[] playerList = roomData.PlayerList;
		foreach (CrumbData crumb in playerList)
		{
			ROOM.AddPlayer(crumb);
		}
	}

	private void OnAddPlayer(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("A:" + packet);
		CrumbData crumb = ConvertTo<CrumbData>(args[0]);
		ROOM.AddPlayer(crumb);
	}

	private void OnRemovePlayer(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("R:" + packet);
		CrumbData crumbData = ConvertTo<CrumbData>(args[0]);
		ROOM.RemovePlayer(crumbData.i);
	}

	private void OnMessage(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("M:" + packet);
		MessageData messageData = ConvertTo<MessageData>(args[0]);
	}

	private void OnUpdatePosition(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("P:" + packet);
	}

	private void OnDestroy()
	{
		manager.Close();
	}

	private void OnServerConnect(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("Connected");
	}

	private void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("Disconnected");
	}

	private void OnError(Socket socket, Packet packet, params object[] args)
	{
		Error error = args[0] as Error;
		switch (error.Code)
		{
		case SocketIOErrors.User:
			Debug.LogWarning("Exception in an event handler!");
			break;
		case SocketIOErrors.Internal:
			Debug.LogWarning("Internal error!");
			break;
		default:
			Debug.LogWarning("server error!");
			break;
		}
	}

	private void OnReconnect(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("Reconnected");
	}

	private void OnReconnecting(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("Reconnecting");
	}

	private void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("ReconnectAttempt");
	}

	private void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
	{
		Debug.Log("ReconnectFailed");
	}
}
