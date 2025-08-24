using UnityEngine;

public class DamageSource : ConfigurableNetworkObject
{
	public float damage = 50f;

	public bool isExplosion = true;

	public bool isMelee;

	public bool ignoreOwner;

	private Collider myCollider;

	private void Awake()
	{
		myCollider = base.collider;
	}

	protected new virtual void Start()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("maxDamage", ref damage, equipmentNames);
		}
	}

	public virtual void OnTriggerEnter(Collider c)
	{
		Physics.IgnoreCollision(myCollider, c);
		dealDamage(c.gameObject);
	}

	public virtual void OnCollisionEnter(Collision c)
	{
		Physics.IgnoreCollision(myCollider, c.collider);
		dealDamage(c.gameObject);
	}

	protected virtual void dealDamage(GameObject target)
	{
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null && (base.OwnerID != damageReceiver.OwnerID || !ignoreOwner))
		{
			damageReceiver.OnTakeDamage(damage * base.DamageMultiplier, base.OwnerID, isExplosion, isMelee, false, true, false, 0f, string.Empty);
		}
	}
}
