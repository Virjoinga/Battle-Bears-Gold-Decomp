using UnityEngine;

public class HealthPack : Powerup
{
	public float healthValue = 20f;

	protected override void Configure(Item item)
	{
		if (item != null)
		{
			item.UpdateProperty("health", ref healthValue, base.EquipmentNames);
		}
	}

	protected override void OnPickup(GameObject obj, PlayerController p)
	{
		p.OnGetHealthPack(obj, healthValue);
	}
}
