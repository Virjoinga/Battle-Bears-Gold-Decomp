using UnityEngine;

public class ProjectileDamageSource : NetworkObject
{
	public float damage;

	public bool isExplosion;

	private Collider myCollider;

	private void Awake()
	{
		myCollider = base.GetComponent<Collider>();
	}

	public void OnTriggerEnter(Collider c)
	{
		if (myCollider != null)
		{
			Physics.IgnoreCollision(c, myCollider);
		}
		dealDamage(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		if (myCollider != null)
		{
			Physics.IgnoreCollision(c.collider, myCollider);
		}
		dealDamage(c.gameObject);
	}

	private void dealDamage(GameObject target)
	{
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null)
		{
			damageReceiver.OnTakeDamage(damage * base.DamageMultiplier, base.OwnerID, isExplosion, false, false, true, false, 0f, string.Empty);
		}
	}
}
