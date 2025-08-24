using ExitGames.Client.Photon;
using UnityEngine;

public class SatelliteDeployableLauncherWeapon : DeployableLauncherWeapon
{
	public override bool OnAttack()
	{
		if (base.NetSyncReporter != null)
		{
			Vector3 position = spawnPoints[0].position;
			Vector3 vector = aimer.forward * projectileSpeed;
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)0] = ownerID;
			hashtable[(byte)1] = position.x;
			hashtable[(byte)2] = position.y;
			hashtable[(byte)3] = position.z;
			hashtable[(byte)4] = vector.x;
			hashtable[(byte)5] = vector.y;
			hashtable[(byte)6] = vector.z;
			base.NetSyncReporter.SetAction(50, hashtable);
		}
		return base.OnAttack();
	}
}
