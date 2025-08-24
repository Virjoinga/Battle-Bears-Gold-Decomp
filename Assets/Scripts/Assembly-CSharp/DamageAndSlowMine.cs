using UnityEngine;

public class DamageAndSlowMine : Mine
{
	private float _damage = 50f;

	private float _radiationDamage;

	public override void ConfigureObject()
	{
		base.ConfigureObject();
		Item itemByName = ServiceManager.Instance.GetItemByName(spawnItemOverride);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("damage", ref _damage, base.EquipmentNames);
			itemByName.UpdateProperty("radiationDamage", ref _radiationDamage, base.EquipmentNames);
		}
	}

	public override void OnDetonateDeployable(PlayerController triggeringPlayer, bool fromExplosion)
	{
		base.OnDetonateDeployable(triggeringPlayer, fromExplosion);
		if (!fromExplosion && triggeringPlayer != null && triggeringPlayer.OwnerID != base.OwnerID)
		{
			if (triggeringPlayer.DamageReceiver != null)
			{
				triggeringPlayer.DamageReceiver.OnTakeDamage(_damage, base.OwnerID, false, false, false, false, false, _radiationDamage, string.Empty);
			}
			if (triggeringPlayer.Motor != null)
			{
				triggeringPlayer.Motor.SetVelocity(Vector3.zero);
			}
		}
	}
}
