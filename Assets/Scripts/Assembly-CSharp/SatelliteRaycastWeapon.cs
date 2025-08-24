using ExitGames.Client.Photon;
using UnityEngine;

public class SatelliteRaycastWeapon : RaycastWeapon
{
	[SerializeField]
	private bool _constantFireSatellite;

	public bool ConstantFireSatellite
	{
		get
		{
			return _constantFireSatellite;
		}
	}

	protected override Vector3 DoRaycastAttack(Vector3 hitPos)
	{
		Vector3 result = base.DoRaycastAttack(hitPos);
		if (isRemote && _constantFireSatellite)
		{
			Debug.Log("Raycasting for remote constant fire");
		}
		if (base.NetSyncReporter != null && !isRemote && !_constantFireSatellite)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)0] = ownerID;
			hashtable[(byte)1] = result.x;
			hashtable[(byte)2] = result.y;
			hashtable[(byte)3] = result.z;
			hashtable[(byte)4] = 0f;
			hashtable[(byte)5] = 0f;
			hashtable[(byte)6] = 0f;
			base.NetSyncReporter.SetAction(50, hashtable);
		}
		return result;
	}
}
