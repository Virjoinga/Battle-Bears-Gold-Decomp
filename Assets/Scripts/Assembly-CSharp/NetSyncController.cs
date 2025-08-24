using System;
using UnityEngine;

public abstract class NetSyncController : MonoBehaviour
{
	protected NetSyncObject netSyncObject;

	public long NetID
	{
		get
		{
			return netSyncObject.NetID;
		}
	}

	private void SetNetSyncObject(NetSyncObject netSyncObject)
	{
		this.netSyncObject = netSyncObject;
	}

	protected virtual void OnCreate()
	{
	}

	public static void Create(Type controllerType, GameObject gameObject, NetSyncObject netSyncObject)
	{
		NetSyncController netSyncController = (NetSyncController)gameObject.AddComponent(controllerType);
		netSyncController.SetNetSyncObject(netSyncObject);
		netSyncObject.SetController(netSyncController);
		netSyncController.OnCreate();
	}
}
