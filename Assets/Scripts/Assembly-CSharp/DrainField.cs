using System.Collections.Generic;
using UnityEngine;

public class DrainField : EffectField
{
	private Dictionary<PlayerController, HealOverTime> heals = new Dictionary<PlayerController, HealOverTime>();

	private Dictionary<PlayerController, Poison> dots = new Dictionary<PlayerController, Poison>();

	private PlayerController Owner;

	private float _healAmount = 2f;

	private float _damageAmount = 1f;

	private float _tickSpeed = 0.5f;

	protected override void Start()
	{
		base.Start();
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(base.OwnerID);
		if (playerCharacterManager != null)
		{
			Owner = playerCharacterManager.PlayerController;
		}
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("healAmount", ref _healAmount, equipmentNames);
			itemByName.UpdateProperty("damage", ref _damageAmount, equipmentNames);
			itemByName.UpdateProperty("healInterval", ref _tickSpeed, equipmentNames);
		}
	}

	protected override void ApplyEffect(PlayerController pc)
	{
		base.ApplyEffect(pc);
		if (Owner != null && Owner != pc)
		{
			if (!Owner.isRemote)
			{
				HealOverTime healOverTime = Owner.gameObject.AddComponent<HealOverTime>();
				healOverTime.duration = _duration;
				healOverTime.healAmount = _healAmount;
				healOverTime.tickSpeed = _tickSpeed;
				heals.Add(pc, healOverTime);
			}
			else if (Owner.Team != pc.Team)
			{
				Poison poison = pc.gameObject.AddComponent<Poison>();
				poison.poisonAmount = _damageAmount;
				poison.poisonDuration = _duration;
				poison.poisonerPlayerID = base.OwnerID;
				poison.showPoisonIcon = false;
				dots.Add(pc, poison);
			}
		}
	}

	protected override void RemoveEffect(PlayerController pc)
	{
		base.RemoveEffect(pc);
		if (heals.ContainsKey(pc))
		{
			Object.Destroy(heals[pc]);
			heals.Remove(pc);
		}
		else if (dots.ContainsKey(pc))
		{
			Object.Destroy(dots[pc]);
			dots.Remove(pc);
		}
	}
}
