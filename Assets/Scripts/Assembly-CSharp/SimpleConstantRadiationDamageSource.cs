using UnityEngine;

public class SimpleConstantRadiationDamageSource : SimpleConstantDamageSource
{
	public float radiationDamage = 15f;

	protected override void Start()
	{
		base.Start();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("radiation", ref radiationDamage, equipmentNames);
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
			DamageReceiver component = target.GetComponent<DamageReceiver>();
			if (component != null && (!_ignoreOwner || base.OwnerID != component.OwnerID))
			{
				float dmg = damage * base.DamageMultiplier * damageInterval;
				float radiationDmg = radiationDamage * damageInterval;
				component.OnTakeDamage(dmg, base.OwnerID, false, false, false, true, false, radiationDmg, string.Empty);
			}
		}
	}
}
