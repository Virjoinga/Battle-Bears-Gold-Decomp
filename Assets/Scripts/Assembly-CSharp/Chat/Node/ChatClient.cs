using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.JSON.LitJson;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.JsonEncoders;
using TextFilter;
using UnityEngine;

namespace Chat.Node
{
	public class ChatClient : ICommunityTextFilter
	{
		private string _serverAddress;

		private SocketManager _manager;

		private ChatRoom _room;

		private string _channel;

		private string _myId;

		private event Action _connected;

		public event Action Connected
		{
			add
			{
				this._connected = (Action)Delegate.Combine(this._connected, value);
			}
			remove
			{
				this._connected = (Action)Delegate.Remove(this._connected, value);
			}
		}

		private event Action<PlayerData> _loggedIn;

		public event Action<PlayerData> LoggedIn
		{
			add
			{
				this._loggedIn = (Action<PlayerData>)Delegate.Combine(this._loggedIn, value);
			}
			remove
			{
				this._loggedIn = (Action<PlayerData>)Delegate.Remove(this._loggedIn, value);
			}
		}

		private event Action _disconnected;

		public event Action Disconnected
		{
			add
			{
				this._disconnected = (Action)Delegate.Combine(this._disconnected, value);
			}
			remove
			{
				this._disconnected = (Action)Delegate.Remove(this._disconnected, value);
			}
		}

		private event Action<string[], bool[]> _subscribedToChannels;

		public event Action<string[], bool[]> SubscribedToChannels
		{
			add
			{
				this._subscribedToChannels = (Action<string[], bool[]>)Delegate.Combine(this._subscribedToChannels, value);
			}
			remove
			{
				this._subscribedToChannels = (Action<string[], bool[]>)Delegate.Remove(this._subscribedToChannels, value);
			}
		}

		private event Action<ChatMessage> _receivedPublicMessage;

		public event Action<ChatMessage> ReceivedPublicMessage
		{
			add
			{
				this._receivedPublicMessage = (Action<ChatMessage>)Delegate.Combine(this._receivedPublicMessage, value);
			}
			remove
			{
				this._receivedPublicMessage = (Action<ChatMessage>)Delegate.Remove(this._receivedPublicMessage, value);
			}
		}

		private event Action<string, string> _playerJoinedChannel;

		public event Action<string, string> PlayerJoinedChannel
		{
			add
			{
				this._playerJoinedChannel = (Action<string, string>)Delegate.Combine(this._playerJoinedChannel, value);
			}
			remove
			{
				this._playerJoinedChannel = (Action<string, string>)Delegate.Remove(this._playerJoinedChannel, value);
			}
		}

		private event Action<string, string> _playerLeftChannel;

		public event Action<string, string> PlayerLeftChannel
		{
			add
			{
				this._playerLeftChannel = (Action<string, string>)Delegate.Combine(this._playerLeftChannel, value);
			}
			remove
			{
				this._playerLeftChannel = (Action<string, string>)Delegate.Remove(this._playerLeftChannel, value);
			}
		}

		public ChatClient(string serverAddress)
		{
			_serverAddress = serverAddress;
			Init();
		}

		~ChatClient()
		{
			Disconnect();
		}

		private void RaiseConnected()
		{
			if (this._connected != null)
			{
				this._connected();
			}
		}

		private void RaiseLoggedIn(PlayerData playerData)
		{
			if (this._loggedIn != null)
			{
				this._loggedIn(playerData);
			}
		}

		private void RaiseDisconnected()
		{
			if (this._disconnected != null)
			{
				this._disconnected();
			}
		}

		private void RaiseSubscribedToChannels(string[] channels, bool[] results)
		{
			if (this._subscribedToChannels != null)
			{
				this._subscribedToChannels(channels, results);
			}
		}

		private void RaiseReceivedPublicMessage(ChatMessage message)
		{
			if (this._receivedPublicMessage != null)
			{
				this._receivedPublicMessage(message);
			}
		}

		private void RaisePlayerJoinedChannel(string channel, string nickname)
		{
			if (this._playerJoinedChannel != null)
			{
				this._playerJoinedChannel(channel, nickname);
			}
		}

		private void RaisePlayerLeftChannel(string channel, string nickname)
		{
			if (this._playerLeftChannel != null)
			{
				this._playerLeftChannel(channel, nickname);
			}
		}

		public void Authenticate(string name, string playerId)
		{
			_myId = playerId;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("name", name);
			dictionary.Add("id", playerId);
			SendEvent("LoginSession", dictionary);
		}

		public void Disconnect()
		{
			_manager.Close();
		}

		public bool PublishMessage(ChatMessage message)
		{
			SendEvent("M", message.Message);
			return true;
		}

		public IList<string> PlayersInChannel(string channel)
		{
			object result;
			if (_room != null)
			{
				IList<string> players = _room.Players;
				result = players;
			}
			else
			{
				result = new List<string>();
			}
			return (IList<string>)result;
		}

		public void FilterText(string text, Action<TextFilterResult> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("Callback to sift text was null. Not going to sift for no reason.");
			}
			_manager.Socket.Once("FilterText", delegate(Socket socket, Packet packet, object[] args)
			{
				callback(ConvertTo<TextFilterResult>(args[0]));
			});
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("text", text);
			SendEvent("FilterText", dictionary);
		}

		public void Ban(string userId, int banLengthInMinutes)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("i", userId);
			dictionary.Add("t", banLengthInMinutes);
			SendEvent("B", dictionary);
		}

		public void ShadowBan(string userId, int banLengthInMinutes)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("i", userId);
			dictionary.Add("t", banLengthInMinutes);
			SendEvent("SB", dictionary);
		}

		public void UnBan(string userId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("i", userId);
			SendEvent("UB", dictionary);
		}

		private void Init()
		{
			SocketOptions socketOptions = new SocketOptions();
			socketOptions.ReconnectionAttempts = 10;
			socketOptions.Reconnection = true;
			socketOptions.Timeout = TimeSpan.FromMilliseconds(5000.0);
			socketOptions.AutoConnect = false;
			_manager = new SocketManager(new Uri(_serverAddress), socketOptions);
			_manager.Encoder = new LitJsonEncoder();
			JsonMapper.RegisterImporter((double input) => (int)(input + 0.5));
			_manager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
			_manager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
			_manager.Socket.On(SocketIOEventTypes.Error, OnError);
			_manager.Socket.On("reconnect", OnReconnect);
			_manager.Socket.On("reconnecting", OnReconnecting);
			_manager.Socket.On("reconnect_attempt", OnReconnectAttempt);
			_manager.Socket.On("reconnect_failed", OnReconnectFailed);
			_manager.Socket.On("Login", OnLogin);
			_manager.Socket.On("JoinRoom", OnJoinRoom);
			_manager.Socket.On("A", OnAddPlayer);
			_manager.Socket.On("R", OnRemovePlayer);
			_manager.Socket.On("M", OnMessage);
			_manager.Open();
		}

		private static string ToJson(object obj)
		{
			JsonWriter jsonWriter = new JsonWriter();
			JsonMapper.ToJson(obj, jsonWriter);
			return jsonWriter.ToString();
		}

		private static T FromJson<T>(string json)
		{
			return JsonMapper.ToObject<T>(json);
		}

		private static T ConvertTo<T>(object obj)
		{
			return FromJson<T>(ToJson(obj));
		}

		private void OnServerConnect(Socket socket, Packet packet, params object[] args)
		{
			RaiseConnected();
		}

		private void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
		{
			RaiseDisconnected();
		}

		private void OnError(Socket socket, Packet packet, params object[] args)
		{
			Error error = args[0] as Error;
			switch (error.Code)
			{
			case SocketIOErrors.User:
				Debug.LogWarning("[ChatClient] Exception in an event handler! " + error.Message);
				break;
			case SocketIOErrors.Internal:
				Debug.LogWarning("[ChatClient] Internal error! " + error.Message);
				break;
			default:
				Debug.LogWarning("[ChatClient] server error! " + error.Message);
				break;
			}
		}

		private void OnReconnect(Socket socket, Packet packet, params object[] args)
		{
		}

		private void OnReconnecting(Socket socket, Packet packet, params object[] args)
		{
		}

		private void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
		{
		}

		private void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
		{
		}

		private void OnLogin(Socket socket, Packet packet, params object[] args)
		{
		}

		private void OnJoinRoom(Socket socket, Packet packet, params object[] args)
		{
			RoomData roomData = ConvertTo<RoomData>(args[0]);
			_room = new ChatRoom(roomData);
			RaiseLoggedIn(new PlayerData(roomData.PlayerList.FirstOrDefault((CrumbData c) => c.i == _myId)));
		}

		private void OnAddPlayer(Socket socket, Packet packet, params object[] args)
		{
			CrumbData crumb = ConvertTo<CrumbData>(args[0]);
			PlayerData playerData = _room.AddPlayer(crumb);
			RaisePlayerJoinedChannel(_channel, playerData.Nickname);
		}

		private void OnRemovePlayer(Socket socket, Packet packet, params object[] args)
		{
			CrumbData crumbData = ConvertTo<CrumbData>(args[0]);
			PlayerData playerData = _room.RemovePlayer(crumbData.i);
			RaisePlayerLeftChannel(_channel, playerData.Nickname);
		}

		private void OnMessage(Socket socket, Packet packet, params object[] args)
		{
			MessageData messageData = ConvertTo<MessageData>(args[0]);
			PlayerData byUniqueId = _room.GetByUniqueId(messageData.i);
			ChatMessage message = new ChatMessage(byUniqueId.Nickname, _channel, messageData);
			RaiseReceivedPublicMessage(message);
		}

		private void SendEvent(string eventName, params object[] args)
		{
			_manager.Socket.Emit(eventName, args);
		}
	}
}
