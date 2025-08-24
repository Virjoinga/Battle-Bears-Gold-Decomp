using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool isLocal;

	public int ID
	{
		get
		{
			return actorID;
		}
	}

	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			if (!isLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
			}
			else
			{
				nameField = value;
			}
		}
	}

	public bool isMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClient == this;
		}
	}

	public Hashtable customProperties { get; private set; }

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(customProperties);
			hashtable[byte.MaxValue] = name;
			return hashtable;
		}
	}

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		InternalCacheProperties(properties);
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties != null && properties.Count != 0 && !customProperties.Equals(properties))
		{
			if (properties.ContainsKey(byte.MaxValue))
			{
				nameField = (string)properties[byte.MaxValue];
			}
			customProperties.Merge(properties);
		}
	}

	public override string ToString()
	{
		return (name != null) ? name : string.Empty;
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && GetHashCode() == photonPlayer.GetHashCode();
	}

	public override int GetHashCode()
	{
		return ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!isLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
		}
		else
		{
			actorID = newID;
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet)
	{
		if (propertiesToSet != null)
		{
			Debug.LogWarning("SetCustomProperties CustomProperties: " + propertiesToSet.ToStringFull());
			customProperties.Merge(propertiesToSet);
			if (actorID > 0)
			{
				PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(actorID, propertiesToSet, true, 0);
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this);
		}
	}

	public static PhotonPlayer Find(int ID)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			if (photonPlayer.ID == ID)
			{
				return photonPlayer;
			}
		}
		return null;
	}

	public void ClearBBRProperties()
	{
		customProperties.Remove((byte)225);
		customProperties.Remove((byte)84);
		customProperties.Remove((byte)86);
		customProperties.Remove((byte)87);
		customProperties.Remove((byte)88);
		customProperties.Remove((byte)89);
		customProperties.Remove((byte)90);
		customProperties.Remove((byte)91);
		customProperties.Remove((byte)92);
		customProperties.Remove((byte)93);
		customProperties.Remove((byte)104);
		customProperties.Remove((byte)106);
		customProperties.Remove((byte)107);
		customProperties.Remove((byte)108);
		customProperties.Remove((byte)109);
		customProperties.Remove((byte)110);
		customProperties.Remove("ready");
		SetCustomProperties(customProperties);
	}
}
