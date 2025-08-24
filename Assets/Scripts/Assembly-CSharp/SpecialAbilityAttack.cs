using UnityEngine;

public class SpecialAbilityAttack : MeleeAttack
{
	public string abilityScriptName;

	public GameObject effect;

	public float abilityDuration = 30f;

	public float cooldownTime = 45f;

	public float maxVisibility = 1f;

	public float minVisibility = 0.25f;

	public bool playEffect = true;

	private float _healAmount;

	private float _healInterval;

	private float _slowAmount;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("abilityDuration", ref abilityDuration, base.EquipmentNames);
		item.UpdateProperty("abilityCooldown", ref cooldownTime, base.EquipmentNames);
		item.UpdateProperty("maxVisibility", ref maxVisibility, base.EquipmentNames);
		item.UpdateProperty("minVisibility", ref minVisibility, base.EquipmentNames);
		item.UpdateProperty("healInterval", ref _healInterval, base.EquipmentNames);
		item.UpdateProperty("healAmount", ref _healAmount, base.EquipmentNames);
		item.UpdateProperty("slowAmount", ref _slowAmount, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	public override bool OnAttack()
	{
		base.OnAttack();
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (base.playerController.gameObject.GetComponent(abilityScriptName) == null)
		{
			if (effect != null && playEffect)
			{
				GameObject gameObject = Object.Instantiate(effect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = myTransform;
				gameObject.transform.localPosition = localOffset;
				gameObject.transform.localScale = Vector3.one;
			}
			SpecialAbility specialAbility = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(base.playerController.gameObject, "Assets/Scripts/Assembly-CSharp/SpecialAbilityAttack.cs (53,36)", abilityScriptName) as SpecialAbility;
			specialAbility.duration = abilityDuration;
			specialAbility.effectPrefab = effect;
			specialAbility.damage = damage;
			if (base.name == "peekaBoo")
			{
				InvisibilityCloak invisibilityCloak = specialAbility as InvisibilityCloak;
				if (invisibilityCloak != null)
				{
					invisibilityCloak.maxAlpha = maxVisibility;
					invisibilityCloak.minAlpha = minVisibility;
				}
			}
			if (specialAbility is SuperSizeMeAbility)
			{
				SuperSizeMeAbility superSizeMeAbility = specialAbility as SuperSizeMeAbility;
				if (superSizeMeAbility != null)
				{
					superSizeMeAbility.HealAmount = _healAmount;
					superSizeMeAbility.HealInterval = _healInterval;
					superSizeMeAbility.SlowPercentage = _slowAmount;
				}
			}
		}
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.OnRemoteAttack(pos, vel, delay);
		if (base.playerController == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (base.playerController.gameObject.GetComponent(abilityScriptName) == null)
		{
			if (effect != null && playEffect)
			{
				GameObject gameObject = Object.Instantiate(effect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = myTransform;
				gameObject.transform.localPosition = localOffset;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			SpecialAbility specialAbility = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(base.playerController.gameObject, "Assets/Scripts/Assembly-CSharp/SpecialAbilityAttack.cs (96,36)", abilityScriptName) as SpecialAbility;
			specialAbility.duration = abilityDuration - (float)delay / 1000f;
			specialAbility.effectPrefab = effect;
		}
	}
}
