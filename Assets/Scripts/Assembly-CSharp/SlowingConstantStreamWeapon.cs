using ExitGames.Client.Photon;
using UnityEngine;

public class SlowingConstantStreamWeapon : ConstantStreamWeapon
{
	[SerializeField]
	private float _slowDuration = 2f;

	[SerializeField]
	private float _slowAmount;

	[SerializeField]
	private float _slowPercentage = 0.25f;

	public override void ConfigureWeapon(Item item)
	{
		base.ConfigureWeapon(item);
		item.UpdateProperty("slowAmount", ref _slowAmount, base.EquipmentNames);
		item.UpdateProperty("speedMultiplier", ref _slowPercentage, base.EquipmentNames);
		item.UpdateProperty("duration", ref _slowDuration, base.EquipmentNames);
	}

	protected override void dealDamage(DamageReceiver d)
	{
		base.dealDamage(d);
		if (d != null)
		{
			PlayerController component = d.GetComponent<PlayerController>();
			if (component != null && component.isRemote)
			{
				SendGetSlowedMessage(component);
			}
		}
	}

	private void SendGetSlowedMessage(PlayerController affectedPlayer)
	{
		if (affectedPlayer.Team != base.playerController.Team && base.NetSyncReporter != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)0] = affectedPlayer.OwnerID;
			hashtable[(byte)1] = _slowDuration;
			hashtable[(byte)2] = _slowAmount;
			hashtable[(byte)3] = _slowPercentage;
			base.NetSyncReporter.SetAction(55, hashtable);
		}
	}
}
