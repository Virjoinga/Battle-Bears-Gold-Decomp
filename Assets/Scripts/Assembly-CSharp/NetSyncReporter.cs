using ExitGames.Client.Photon;
using UnityEngine;

public class NetSyncReporter : MonoBehaviour
{
	private int lastReportTime = int.MinValue;

	private GameObject target;

	private long netID = -1L;

	private Transform angleTransform;

	public long NetID
	{
		get
		{
			return netID;
		}
	}

	private void Start()
	{
		lastReportTime = PhotonManager.Instance.ServerTimeInMilliseconds - 10000;
	}

	public void SetTarget(GameObject gameObject, long netID)
	{
		target = gameObject;
		this.netID = netID;
		angleTransform = (target.GetComponentInChildren(typeof(AngleControllerPlaceholder)) as AngleControllerPlaceholder).transform;
	}

	public void OnNetTick(int serverTime)
	{
		if (netID != -1 && !NetSyncManager.Instance.Objects[netID].IsStatic && serverTime - lastReportTime > 0)
		{
			PhotonManager.Instance.ReportTransformUpdate(netID, target, angleTransform);
			lastReportTime = serverTime;
		}
	}

	public void SetAction(byte action, Hashtable parameters)
	{
		if (PhotonManager.Instance != null)
		{
			PhotonManager.Instance.ReportActionUpdate(netID, action, parameters);
		}
	}

	public void SpawnProjectile(Vector3 pos, Vector3 vel)
	{
		PhotonManager.Instance.ReportFireProjectileUpdate(netID, pos, vel);
	}
}
