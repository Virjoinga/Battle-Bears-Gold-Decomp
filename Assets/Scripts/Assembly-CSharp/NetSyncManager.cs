using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class NetSyncManager : MonoBehaviour
{
	private static NetSyncManager instance;

	private Dictionary<long, NetSyncListener> listeners = new Dictionary<long, NetSyncListener>();

	private Dictionary<long, NetSyncReporter> reporters = new Dictionary<long, NetSyncReporter>();

	private Dictionary<long, NetSyncObject> netSyncObjects = new Dictionary<long, NetSyncObject>();

	private Dictionary<string, NetSyncServerObject> pendingServerObjects = new Dictionary<string, NetSyncServerObject>();

	private Dictionary<string, NetSyncServerObject> serverObjectLookup = new Dictionary<string, NetSyncServerObject>();

	private long currentNetSyncID;

	public long CurrentNetSyncID
	{
		get
		{
			return ++currentNetSyncID;
		}
		set
		{
			currentNetSyncID = value;
		}
	}

	public static NetSyncManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (NetSyncManager)Object.FindObjectOfType(typeof(NetSyncManager));
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	public Dictionary<long, NetSyncListener> Listeners
	{
		get
		{
			return listeners;
		}
	}

	public Dictionary<long, NetSyncReporter> Reporters
	{
		get
		{
			return reporters;
		}
	}

	public Dictionary<long, NetSyncObject> Objects
	{
		get
		{
			return netSyncObjects;
		}
	}

	public Dictionary<string, NetSyncServerObject> ServerObjectLookup
	{
		get
		{
			return serverObjectLookup;
		}
	}

	public void CleanUp()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)72, currentNetSyncID);
		PhotonNetwork.networkingPeer.OpRaiseEvent(94, hashtable, true, 0);
		List<long> list = new List<long>();
		foreach (long key in Objects.Keys)
		{
			list.Add(key);
		}
		foreach (long item in list)
		{
			UnregisterNetSyncObject(item);
		}
		list.Clear();
	}

	public bool ContainsNetSyncObject(long id)
	{
		return netSyncObjects.ContainsKey(id);
	}

	public void RegisterNetSyncObject(long netID, int ownerID, NetSyncObject netSyncObject, GameObject obj)
	{
		if (!netSyncObjects.ContainsKey(netID))
		{
			netSyncObjects.Add(netID, netSyncObject);
		}
		if (obj != null)
		{
			if (ownerID != PhotonManager.Instance.LocalUserID)
			{
				NetSyncListener listener = (NetSyncListener)obj.AddComponent(typeof(NetSyncListener));
				RegisterNetSyncListener(netID, obj, netSyncObject, listener);
			}
			else
			{
				NetSyncReporter reporter = (NetSyncReporter)obj.AddComponent(typeof(NetSyncReporter));
				RegisterNetSyncReporter(netID, obj, reporter);
			}
		}
	}

	public void RegisterPendingNetSyncServerObject(NetSyncServerObject netSyncServerObject)
	{
		pendingServerObjects.Add(netSyncServerObject.objectName, netSyncServerObject);
	}

	public void RegisterNetSyncServerObject(long netID, string name, int state)
	{
		if (pendingServerObjects.ContainsKey(name))
		{
			pendingServerObjects[name].RegisterNetID(netID);
			NetSyncObject netSyncObject = pendingServerObjects[name];
			pendingServerObjects.Remove(name);
			RegisterNetSyncObject(netSyncObject.NetID, netSyncObject.OwnerID, netSyncObject, netSyncObject.gameObject);
			serverObjectLookup.Add(name, netSyncObject as NetSyncServerObject);
			netSyncObject.SetState(state);
		}
	}

	public void UnregisterNetSyncObject(long id)
	{
		if (netSyncObjects.ContainsKey(id))
		{
			if (netSyncObjects[id] is NetSyncServerObject)
			{
				serverObjectLookup.Remove((netSyncObjects[id] as NetSyncServerObject).objectName);
			}
			netSyncObjects[id].DestroyObject();
			Object.Destroy(netSyncObjects[id]);
			netSyncObjects.Remove(id);
		}
		if (listeners.ContainsKey(id))
		{
			UnregisterNetSyncListener(id);
		}
		if (reporters.ContainsKey(id))
		{
			UnregisterNetSyncReporter(id);
		}
	}

	private void RegisterNetSyncListener(long id, GameObject gameObject, NetSyncObject nso, NetSyncListener listener)
	{
		if (!listeners.ContainsKey(id))
		{
			listeners.Add(id, listener);
		}
		else
		{
			listeners[id] = listener;
		}
		listener.SetTarget(gameObject, nso);
	}

	private void UnregisterNetSyncListener(long id)
	{
		if (listeners.ContainsKey(id))
		{
			Object.DestroyImmediate(listeners[id]);
			listeners.Remove(id);
		}
	}

	private void RegisterNetSyncReporter(long id, GameObject gameObject, NetSyncReporter reporter)
	{
		if (!reporters.ContainsKey(id))
		{
			reporters.Add(id, reporter);
		}
		else
		{
			reporters[id] = reporter;
		}
		reporter.SetTarget(gameObject, id);
	}

	private void UnregisterNetSyncReporter(long id)
	{
		if (reporters.ContainsKey(id))
		{
			Object.DestroyImmediate(reporters[id]);
			reporters.Remove(id);
		}
	}

	public void DispatchNetSyncTransform(long netID, float posX, float posY, float posZ, float baseRotY, float angleRotX, int timestamp)
	{
		if (listeners.ContainsKey(netID))
		{
			TransformPackage transformPackage = new TransformPackage();
			transformPackage.position = new Vector3(posX, posY, posZ);
			transformPackage.baseYrotation = baseRotY;
			transformPackage.angleXRotation = angleRotX;
			transformPackage.timestamp = timestamp;
			listeners[netID].AddTransformPackage(transformPackage);
		}
	}

	public void DispatchNetSyncState(long netID, int state, int timestamp)
	{
		if (listeners.ContainsKey(netID))
		{
			StatePackage statePackage = new StatePackage();
			statePackage.state = state;
			statePackage.timestamp = timestamp;
			listeners[netID].AddStatePackage(statePackage);
		}
	}

	public void DispatchNetSyncAction(long netID, byte action, Hashtable parameters, int timestamp)
	{
		if (listeners.ContainsKey(netID))
		{
			ActionPackage actionPackage = new ActionPackage();
			actionPackage.action = action;
			actionPackage.parameters = parameters;
			actionPackage.timestamp = timestamp;
			listeners[netID].AddActionPackage(actionPackage);
		}
	}

	public void DispatchSyncFireProjectile(long netID, float posX, float posY, float posZ, float velX, float velY, float velZ, int timestamp)
	{
		if (listeners.ContainsKey(netID))
		{
			FireProjectilePackage fireProjectilePackage = new FireProjectilePackage();
			fireProjectilePackage.position = new Vector3(posX, posY, posZ);
			fireProjectilePackage.velocity = new Vector3(velX, velY, velZ);
			fireProjectilePackage.timestamp = timestamp;
			listeners[netID].AddFireProjectilePackage(fireProjectilePackage);
		}
	}

	public void OnNetTick(int serverTime)
	{
		foreach (NetSyncReporter value in reporters.Values)
		{
			value.OnNetTick(serverTime);
		}
	}
}
