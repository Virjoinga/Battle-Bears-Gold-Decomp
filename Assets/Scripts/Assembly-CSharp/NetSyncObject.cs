using ExitGames.Client.Photon;
using UnityEngine;

public abstract class NetSyncObject : MonoBehaviour
{
	protected long netID;

	private int ownerID;

	protected int state;

	protected GameObject obj;

	private NetSyncController controller;

	protected bool isStatic;

	public long NetID
	{
		get
		{
			return netID;
		}
	}

	public int OwnerID
	{
		get
		{
			return ownerID;
		}
	}

	public GameObject GameObject
	{
		get
		{
			return obj;
		}
	}

	public int State
	{
		get
		{
			return state;
		}
	}

	public bool IsStatic
	{
		get
		{
			return isStatic;
		}
		set
		{
			isStatic = value;
		}
	}

	public NetSyncController Controller
	{
		get
		{
			return controller;
		}
	}

	public void Construct(long netID, Vector3 startPos, float startBaseY, float startAngleX, int startState, int ownerID)
	{
		this.netID = netID;
		this.ownerID = ownerID;
		state = startState;
		obj = CreateObject(startPos, startBaseY, startAngleX);
		NetSyncManager.Instance.RegisterNetSyncObject(netID, ownerID, this, obj);
		HandleStateChange(startState);
		PostCreate();
	}

	public void SetController(NetSyncController controller)
	{
		this.controller = controller;
	}

	public abstract GameObject CreateObject(Vector3 startPos, float startBaseY, float startAngleX);

	public abstract void PostCreate();

	public abstract void DestroyObject();

	public abstract bool HandleStateChange(int state);

	public abstract bool HandleActionChange(byte action, Hashtable parameters, int delay);

	public abstract bool HandleFireProjectile(Vector3 pos, Vector3 vel, int delay);

	public bool SetState(int state)
	{
		if (HandleStateChange(state))
		{
			this.state = state;
			PhotonManager.Instance.ReportStateUpdate(netID, state);
			return true;
		}
		return false;
	}

	public void SetAction(byte action, Hashtable parameters)
	{
		PhotonManager.Instance.ReportActionUpdate(netID, action, parameters);
	}
}
