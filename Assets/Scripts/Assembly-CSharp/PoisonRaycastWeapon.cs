using ExitGames.Client.Photon;

public class PoisonRaycastWeapon : RaycastWeapon
{
	private float _poisonDuration = 10f;

	public override void ConfigureWeapon(Item item)
	{
		base.ConfigureWeapon(item);
		item.UpdateProperty("duration", ref _poisonDuration, base.EquipmentNames);
	}

	protected override void OnHit(DamageReceiver d)
	{
		if (d != null)
		{
			PlayerController component = d.gameObject.GetComponent<PlayerController>();
			if (component != null)
			{
				OnDealDirectDamage(d, 0f);
				SendPoisonMessage(component);
			}
		}
	}

	private void SendPoisonMessage(PlayerController pc)
	{
		byte action = 61;
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)0] = pc.OwnerID;
		hashtable[(byte)1] = damage;
		hashtable[(byte)2] = ownerID;
		hashtable[(byte)3] = _poisonDuration;
		if (!dontSendNetworkMessages)
		{
			base.NetSyncReporter.SetAction(action, hashtable);
		}
	}
}
