using System.Collections.Generic;
using System.Linq;

namespace Chat
{
	public class ChatRoom
	{
		private Dictionary<string, PlayerData> _playersByUniqueId;

		public IList<string> Players
		{
			get
			{
				return _playersByUniqueId.Values.Select((PlayerData p) => p.Nickname).ToList();
			}
		}

		public string Id { get; private set; }

		public ChatRoom(RoomData data)
		{
			Id = data.RoomId;
			_playersByUniqueId = new Dictionary<string, PlayerData>();
			CrumbData[] playerList = data.PlayerList;
			foreach (CrumbData crumb in playerList)
			{
				AddPlayer(crumb);
			}
		}

		public PlayerData AddPlayer(CrumbData crumb)
		{
			PlayerData playerData = new PlayerData(crumb);
			if (!_playersByUniqueId.ContainsKey(playerData.PlayerId))
			{
				_playersByUniqueId.Add(playerData.PlayerId, playerData);
			}
			return playerData;
		}

		public PlayerData RemovePlayer(string playerId)
		{
			PlayerData value;
			if (_playersByUniqueId.TryGetValue(playerId, out value))
			{
				_playersByUniqueId.Remove(playerId);
			}
			return value;
		}

		public PlayerData GetByUniqueId(string playFabId)
		{
			PlayerData value = null;
			_playersByUniqueId.TryGetValue(playFabId, out value);
			return value;
		}
	}
}
