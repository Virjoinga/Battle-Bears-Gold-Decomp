using System.Collections;
using UnityEngine;

public class InstantaneousDamageSource : ConfigurableNetworkObject
{
	public float minDamage = 10f;

	public float maxDamage = 50f;

	public float minDamageDistance = 80f;

	public float maxDamageDistance = 25f;

	public bool isExplosion = true;

	public bool isMelee;

	private Collider myCollider;

	public bool IgnoreOwner;

	private void Awake()
	{
		myCollider = base.collider;
	}

	protected override void Start()
	{
		base.Start();
		if (configureItemName != null && configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			if (itemByName != null)
			{
				itemByName.UpdateProperty("minDamage", ref minDamage, equipmentNames);
				itemByName.UpdateProperty("maxDamage", ref maxDamage, equipmentNames);
				itemByName.UpdateProperty("minDamageRange", ref minDamageDistance, equipmentNames);
				itemByName.UpdateProperty("maxDamageRange", ref maxDamageDistance, equipmentNames);
			}
		}
		if (myCollider != null && myCollider is SphereCollider)
		{
			SphereCollider sphereCollider = (SphereCollider)myCollider;
			sphereCollider.radius = minDamageDistance;
		}
		StartCoroutine(delayedSelfDestruct());
	}

	public void OnTriggerEnter(Collider c)
	{
		Physics.IgnoreCollision(myCollider, c);
		dealDamage(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		Physics.IgnoreCollision(myCollider, c.collider);
		dealDamage(c.gameObject);
	}

	protected virtual void dealDamage(GameObject target)
	{
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null && (!IgnoreOwner || damageReceiver.OwnerID != base.OwnerID))
		{
			float num = Vector3.Distance(target.transform.position, base.transform.position);
			float num2 = 0f;
			num2 = ((num > minDamageDistance) ? minDamage : ((!(num < maxDamageDistance)) ? (minDamage + (maxDamage - minDamage) * (1f - (num - maxDamageDistance) / minDamageDistance)) : maxDamage));
			damageReceiver.OnTakeDamage(num2 * base.DamageMultiplier, base.OwnerID, isExplosion, isMelee, false, true, false, 0f, string.Empty);
		}
	}

	protected IEnumerator delayedSelfDestruct()
	{
		yield return new WaitForSeconds(0.1f);
		Object.Destroy(this);
	}
}
