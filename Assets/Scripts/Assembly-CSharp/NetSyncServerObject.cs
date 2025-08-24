using UnityEngine;

public abstract class NetSyncServerObject : NetSyncObject
{
	public string objectName = "NetSyncServerObject";

	private void Awake()
	{
		OnInit();
	}

	protected virtual void OnInit()
	{
		isStatic = true;
	}

	public void RegisterNetID(long netID)
	{
		base.netID = netID;
	}

	public override GameObject CreateObject(Vector3 startPos, float startBaseY, float startAngleX)
	{
		return null;
	}

	public override void PostCreate()
	{
	}
}
