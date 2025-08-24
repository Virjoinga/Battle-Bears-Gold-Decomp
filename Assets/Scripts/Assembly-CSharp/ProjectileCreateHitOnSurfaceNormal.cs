using UnityEngine;

public class ProjectileCreateHitOnSurfaceNormal : Projectile
{
	public AudioClip[] impactSounds;

	protected override void OnTriggerEnter(Collider c)
	{
	}

	protected override void OnCollisionEnter(Collision c)
	{
		ExplodeOrientedToNormal(null, c);
	}

	public void ExplodeOrientedToNormal(GameObject objectHit, Collision collisionInfo)
	{
		if (objectToSpawn != null && !hasSpawned)
		{
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, collisionInfo.contacts[0].normal);
			GameObject gameObject = Object.Instantiate(objectToSpawn.gameObject, collisionInfo.contacts[0].point, rotation) as GameObject;
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.OwnerID = base.OwnerID;
			component.DamageMultiplier = base.DamageMultiplier;
			component.SetItemOverride(spawnItemOverride);
			component.SetEquipmentNames(equipmentNames);
			hasSpawned = true;
			if (impactSounds.Length > 0)
			{
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.position = base.transform.position;
				AudioSource audioSource = gameObject2.AddComponent<AudioSource>();
				audioSource.maxDistance = 2000f;
				audioSource.clip = impactSounds[Random.Range(0, impactSounds.Length)];
				audioSource.loop = false;
				audioSource.Play();
				Object.Destroy(gameObject2, audioSource.clip.length);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
