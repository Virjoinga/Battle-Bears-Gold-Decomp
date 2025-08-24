using System.Collections;
using UnityEngine;

public class Lobby : MonoBehaviour
{
	private RoomInfo[] roomsAvailable;

	private void Awake()
	{
		PhotonNetwork.ConnectUsingSettings("1.4");
		StartCoroutine(CheckIfJoinedLobby());
		Application.runInBackground = true;
	}

	private IEnumerator CheckIfJoinedLobby()
	{
		int timeOut = 30;
		while (PhotonNetwork.connectionStateDetailed != PeerState.JoinedLobby && timeOut > 0)
		{
			timeOut--;
			yield return new WaitForSeconds(0.5f);
		}
		if (PhotonNetwork.connectionStateDetailed == PeerState.JoinedLobby)
		{
			Debug.Log("JointedLobby now : spend time = " + (float)(30 - timeOut) * 0.5f + " seconds");
			roomsAvailable = PhotonNetwork.GetRoomList();
			Debug.Log(roomsAvailable.Length);
		}
		else if (timeOut <= 0 && PhotonNetwork.connectionStateDetailed != PeerState.JoinedLobby)
		{
			Debug.Log("failed to Connect : time out");
		}
		else
		{
			Debug.Log("failed to Connect : " + PhotonNetwork.connectionStateDetailed);
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect((float)Screen.width * 0.5f, 30f, 100f, 100f), PhotonNetwork.connectionStateDetailed.ToString());
		string empty = string.Empty;
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
		{
			GUI.Label(new Rect((float)Screen.width * 0.5f, 130f, 100f, 100f), "PlayerCounts in room\n" + PhotonNetwork.countOfPlayersInRooms);
			if (GUI.Button(new Rect((float)Screen.width - 100f, 0f, 100f, 49f), "Leave Room"))
			{
				PhotonNetwork.LeaveRoom();
			}
			if (PhotonNetwork.isMasterClient && GUI.Button(new Rect((float)Screen.width - 100f, 50f, 100f, 49f), "Play"))
			{
				GetComponent<PhotonView>().RPC("LoadLevel", PhotonTargets.AllBuffered);
			}
		}
		else
		{
			if (GUI.Button(new Rect((float)Screen.width - 100f, 0f, 100f, 49f), "Refresh List"))
			{
				roomsAvailable = PhotonNetwork.GetRoomList();
			}
			if (GUI.Button(new Rect((float)Screen.width - 100f, 50f, 100f, 49f), "Create room"))
			{
				PhotonNetwork.CreateRoom(string.Empty, true, true, 8);
			}
		}
		if (roomsAvailable == null || roomsAvailable.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < roomsAvailable.Length; i++)
		{
			if (GUI.Button(new Rect(0f, 50f * (float)i, 100f, 49f), roomsAvailable[i].name))
			{
				PhotonNetwork.JoinRoom(roomsAvailable[i].name);
			}
		}
	}

	[RPC]
	private void LoadLevel()
	{
		Application.LoadLevel("AbusementPark");
		PhotonNetwork.isMessageQueueRunning = false;
	}
}
