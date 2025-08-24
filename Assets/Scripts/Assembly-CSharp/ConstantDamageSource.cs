using System.Collections.Generic;
using UnityEngine;

public class ConstantDamageSource : ConfigurableNetworkObject
{
	[SerializeField]
	protected bool _ignoreOwner;

	private float minDamage = 10f;

	private float maxDamage = 50f;

	private float minDamageDistance = 80f;

	private float maxDamageDistance = 25f;

	public bool isExplosion = true;

	public bool isMelee;

	protected float damageInterval = 0.25f;

	protected Dictionary<GameObject, float> lastDamageTimes = new Dictionary<GameObject, float>();

	protected override void Start()
	{
		base.Start();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("minDamage", ref minDamage, equipmentNames);
			itemByName.UpdateProperty("maxDamage", ref maxDamage, equipmentNames);
			itemByName.UpdateProperty("minDamageRange", ref minDamageDistance, equipmentNames);
			itemByName.UpdateProperty("maxDamageRange", ref maxDamageDistance, equipmentNames);
		}
		if (base.collider != null && base.collider is SphereCollider && minDamageDistance != 80f)
		{
			SphereCollider sphereCollider = (SphereCollider)base.collider;
			sphereCollider.radius = minDamageDistance;
		}
	}

	public void OnTriggerEnter(Collider c)
	{
		dealDamage(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		dealDamage(c.gameObject);
	}

	public void OnTriggerStay(Collider c)
	{
		dealDamage(c.gameObject);
	}

	public void OnCollisionStay(Collision c)
	{
		dealDamage(c.gameObject);
	}

	protected virtual void dealDamage(GameObject target)
	{
		if (!lastDamageTimes.ContainsKey(target) || Time.time > lastDamageTimes[target] + damageInterval)
		{
			if (!lastDamageTimes.ContainsKey(target))
			{
				lastDamageTimes.Add(target, Time.time);
			}
			else
			{
				lastDamageTimes[target] = Time.time;
			}
			DamageReceiver component = target.GetComponent<DamageReceiver>();
			if (component != null && (!_ignoreOwner || base.OwnerID != component.OwnerID))
			{
				float num = Vector3.Distance(target.transform.position, base.transform.position);
				float num2 = 0f;
				num2 = ((num > minDamageDistance) ? minDamage : ((!(num < maxDamageDistance)) ? (minDamage + (maxDamage - minDamage) * (1f - (num - maxDamageDistance) / minDamageDistance)) : maxDamage));
				num2 *= base.DamageMultiplier * damageInterval * base.MeleeMultiplier;
				component.OnTakeDamage(num2, base.OwnerID, isExplosion, isMelee, false, true, false, 0f, string.Empty);
			}
		}
	}
}
