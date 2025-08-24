using ExitGames.Client.Photon;
using UnityEngine;

public class FreezingInstantaneousDamageSource : InstantaneousDamageSource
{
	private float _freezeTime = 5f;

	protected override void Start()
	{
		base.Start();
		Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("duration", ref _freezeTime, equipmentNames);
		}
	}

	protected override void dealDamage(GameObject target)
	{
		base.dealDamage(target);
		PlayerController component = target.GetComponent<PlayerController>();
		if (component != null && !component.isRemote)
		{
			Team team = GameManager.Instance.Players(base.OwnerID).team;
			if (component.Team != team || (component.OwnerID == base.OwnerID && !IgnoreOwner))
			{
				component.Freeze(_freezeTime);
				SendFreezeMessage(component);
			}
		}
	}

	private void SendFreezeMessage(PlayerController pc)
	{
		byte action = 62;
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)0] = pc.OwnerID;
		hashtable[(byte)1] = _freezeTime;
		pc.NetSync.SetAction(action, hashtable);
	}
}
