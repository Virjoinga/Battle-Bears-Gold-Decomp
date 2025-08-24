using UnityEngine;

public class JoulesPack : Powerup
{
	public int joulesValue = 25;

	public int duration = 20;

	public int OwnerID { get; set; }

	public int Index { get; set; }

	protected override void Start()
	{
		base.Start();
		Item itemByName = ServiceManager.Instance.GetItemByName(base.name);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("duration", ref duration, "|");
		}
		Object.Destroy(base.gameObject, duration);
	}

	protected override void Configure(Item item)
	{
		item.UpdateProperty("joules", ref joulesValue, base.EquipmentNames);
	}

	protected override void OnPickup(GameObject obj, PlayerController p)
	{
		p.OnGetJoulesPack(obj, joulesValue);
	}
}
