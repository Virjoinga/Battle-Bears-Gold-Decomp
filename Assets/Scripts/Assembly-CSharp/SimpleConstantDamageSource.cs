using UnityEngine;

public class SimpleConstantDamageSource : ConstantDamageSource
{
	public float damage;

	protected override void Start()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("damage", ref damage, equipmentNames);
		}
	}

	protected override void dealDamage(GameObject target)
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
			DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (damageReceiver != null && (!_ignoreOwner || base.OwnerID != damageReceiver.OwnerID))
			{
				damageReceiver.OnTakeDamage(damage * base.DamageMultiplier * damageInterval, base.OwnerID, isExplosion, false, false, true, false, 0f, string.Empty);
			}
		}
	}
}
