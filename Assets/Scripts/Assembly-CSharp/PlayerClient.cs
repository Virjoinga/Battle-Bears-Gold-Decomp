using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class PlayerClient : Photon.MonoBehaviour, IPhotonClient
{
	public static string userName = string.Empty;

	public void Start()
	{
		PhotonManager.Instance.RegisterClient(this, typeof(PlayerUser));
		PhotonManager.Instance.OnUserCreated += OnUserCreated;
		PhotonManager.Instance.OnConnected += OnConnected;
		PhotonManager.Instance.OnStartPlaying += OnStartPlaying;
		string text = ServiceManager.Instance.GetPhotonAppID();
		if (string.IsNullOrEmpty(text))
		{
			text = "b2f1c620-3f72-4bc6-a80a-eeabcb8a948c";
		}
		PhotonManager.Instance.Connect(text, ServiceManager.Instance.GetMatchGameServer());
		if (PhotonNetwork.connected)
		{
			Debug.LogWarning("already connected, running OnJoinedLobby");
			OnJoinedLobby();
		}
	}

	public void IngameUpdate()
	{
	}

	public void NetworkTick(int serverTime)
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.TimeUpdate(serverTime);
		}
	}

	public void HandleCustomEvent(byte evCode, ref Hashtable data, int senderID)
	{
	}

	public void HandleCustomPeerReturn(byte opCode, int returnCode, ref Hashtable returnValues, short invocID)
	{
	}

	public void HandleGetProperties(Hashtable properties, short invocID)
	{
	}

	private void OnStartPlaying(object sender, OnStartPlayingEventArgs e)
	{
	}

	private void OnUserCreated(object sender, OnUserCreatedEventArgs e)
	{
		if (e.isLocalUser)
		{
			Vector3 position = new Vector3(0f, 5555f, -500f);
			PhotonManager.Instance.RegisterNetSyncObject(typeof(PlayerCharacterManager).ToString(), position, 0f, 0f, 0);
		}
	}

	private void OnConnected(object obj, OnConnectedEventArgs connectionArgs)
	{
		string[] array = ServiceManager.Instance.GetMatchGameName().Split(':');
		PhotonManager.Instance.MAX_PLAYERS = array.Length - 2;
		Hashtable photonUserParametersForCurrentLoadout = LoadoutManager.Instance.GetPhotonUserParametersForCurrentLoadout();
		PhotonManager.Instance.Join(ServiceManager.Instance.GetMatchGameName(), photonUserParametersForCurrentLoadout);
	}

	public void OnJoinedLobby()
	{
		Debug.Log("PlayerClient joined the lobby");
		OnConnected(this, null);
	}
}
