using UnityEngine;

public class InteractionSpawnObject : InteractionPointBase
{
	[SerializeField]
	protected GameObject objectToSpawn;

	[SerializeField]
	protected float cooldownTime;

	[SerializeField]
	protected bool destroyOnSpawn;

	[SerializeField]
	protected Vector3 spawnPoint;

	protected float _lastSpawnTime = -1f;

	protected InteractionPointDamagerReciever _damageReceiver;

	public void Awake()
	{
		_damageReceiver = GetComponent<InteractionPointDamagerReciever>();
	}

	public override void InteractionPointTriggered(int characterIndex)
	{
		if (Time.time > _lastSpawnTime + cooldownTime)
		{
			GameObject gameObject = Object.Instantiate(objectToSpawn, spawnPoint, Quaternion.identity) as GameObject;
			Collider componentInChildren = base.gameObject.GetComponentInChildren<Collider>();
			Collider componentInChildren2 = gameObject.GetComponentInChildren<Collider>();
			if (componentInChildren != null && componentInChildren2 != null)
			{
				Physics.IgnoreCollision(componentInChildren, componentInChildren2);
			}
			Projectile componentInChildren3 = gameObject.GetComponentInChildren<Projectile>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.OwnerID = characterIndex;
			}
			_lastSpawnTime = Time.time;
			if (_damageReceiver != null)
			{
				_damageReceiver.Respawn();
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(spawnPoint, 10f);
	}
}
