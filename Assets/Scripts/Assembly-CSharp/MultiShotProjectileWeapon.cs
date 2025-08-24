using UnityEngine;

public class MultiShotProjectileWeapon : ProjectileWeapon
{
	[SerializeField]
	private bool _hideWeaponsOnDeath;

	public override bool OnAttack()
	{
		PerformLocalAttackAnimationAndEffects();
		bool result = true;
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			spawnIndex = i;
			GameObject gameObject = SpawnProjectile(spawnPoints[spawnIndex].position, DirectionFromReticleRaycast(spawnIndex) * projectileSpeed);
			DelayedGravityProjectile component = gameObject.GetComponent<DelayedGravityProjectile>();
			if (component != null)
			{
				component.ownerID = base.OwnerID;
			}
			if (!isRemote && base.NetSyncReporter != null && !dontSendNetworkMessages)
			{
				base.NetSyncReporter.SpawnProjectile(gameObject.transform.position, gameObject.GetComponent<Rigidbody>().velocity);
			}
			gameObject.SendMessage("OnNetworkDelay", 0f, SendMessageOptions.DontRequireReceiver);
		}
		return result;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		SpawnRemoteProjectile(pos, vel, delay);
	}

	public override void WeaponDeath()
	{
		base.WeaponDeath();
		if (_hideWeaponsOnDeath)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = false;
			}
		}
	}
}
