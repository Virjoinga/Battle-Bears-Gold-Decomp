using UnityEngine;

public class PowerGlove : Powerup
{
	public float damageMultiplier = 2f;

	public float duration = 20f;

	protected override void Configure(Item item)
	{
		if (item != null)
		{
			item.UpdateProperty("damageMultiplier", ref damageMultiplier, base.EquipmentNames);
			item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		}
	}

	protected override void OnPickup(GameObject obj, PlayerController p)
	{
		p.OnGetPowerGlove(obj, damageMultiplier, duration);
	}
}
