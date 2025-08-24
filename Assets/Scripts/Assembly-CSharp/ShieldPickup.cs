using UnityEngine;

public class ShieldPickup : Powerup
{
	public float duration = 20f;

	protected override void OnPickup(GameObject obj, PlayerController p)
	{
		p.OnGetShield(obj, duration);
	}

	protected override void Configure(Item item)
	{
		if (item != null)
		{
			item.UpdateProperty("duration", ref duration, base.EquipmentNames);
		}
	}
}
