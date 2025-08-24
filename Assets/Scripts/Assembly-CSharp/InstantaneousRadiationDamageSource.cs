using UnityEngine;

public class InstantaneousRadiationDamageSource : InstantaneousDamageSource
{
	public float minRadiation = 30f;

	public float maxRadiation = 40f;

	protected override void Start()
	{
		base.Start();
		if (configureItemName != null && configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			if (itemByName != null)
			{
				itemByName.UpdateProperty("minRadiation", ref minRadiation, equipmentNames);
				itemByName.UpdateProperty("maxRadiation", ref maxRadiation, equipmentNames);
			}
		}
	}

	protected override void dealDamage(GameObject target)
	{
		DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null && (!IgnoreOwner || damageReceiver.OwnerID != base.OwnerID))
		{
			float num = Vector3.Distance(target.transform.position, base.transform.position);
			float num2 = 0f;
			float num3 = 0f;
			if (num > minDamageDistance)
			{
				num2 = minDamage;
				num3 = minRadiation;
			}
			else if (num < maxDamageDistance)
			{
				num2 = maxDamage;
				num3 = maxRadiation;
			}
			else
			{
				num2 = minDamage + (maxDamage - minDamage) * (1f - (num - maxDamageDistance) / minDamageDistance);
				num3 = minRadiation + (maxRadiation - minRadiation) * (1f - (num - maxDamageDistance) / minDamageDistance);
			}
			damageReceiver.OnTakeDamage(num2 * base.DamageMultiplier, base.OwnerID, isExplosion, isMelee, false, true, false, num3, string.Empty);
		}
	}
}
