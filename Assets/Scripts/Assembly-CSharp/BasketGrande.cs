using UnityEngine;

public class BasketGrande : Powerup
{
	public float healthPercentGain = 50f;

	protected override void Configure(Item item)
	{
		if (item != null)
		{
			item.UpdateProperty("healthPercent", ref healthPercentGain, base.EquipmentNames);
		}
	}

	protected override void OnPickup(GameObject obj, PlayerController p)
	{
		if (p != null && obj != null)
		{
			p.OnGetBasketGrande(obj, healthPercentGain);
		}
	}
}
