using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	[Serializable]
	public class Player
	{
		public bool isLocal;

		public string PlayerId;

		public string Nickname;
	}

	private PlayerManager PLAYER;

	public string RoomId;

	public List<Player> PlayerList;

	private void Awake()
	{
		PLAYER = GetComponent<PlayerManager>();
	}

	public void JoinRoom(string roomId)
	{
		RoomId = roomId;
		PlayerList = new List<Player>();
	}

	public void AddPlayer(CrumbData crumb)
	{
		Player player = new Player();
		player.PlayerId = crumb.i;
		player.Nickname = crumb.n;
		if (player.PlayerId == PLAYER.PlayerId)
		{
			player.isLocal = true;
		}
		PlayerList.Add(player);
	}

	public void RemovePlayer(string playerId)
	{
		Player item = PlayerList.Find((Player p) => p.PlayerId == playerId);
		PlayerList.Remove(item);
	}

	public void MovePlayer(string playerId, int x, int y)
	{
		Player player = PlayerList.Find((Player p) => p.PlayerId == playerId);
	}
}
