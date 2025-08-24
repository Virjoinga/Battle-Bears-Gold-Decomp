﻿using UnityEngine;

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
                GameObject go = Object.Instantiate(effect, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.parent = myTransform;
                go.transform.localPosition = localOffset;
                go.transform.localScale = Vector3.one;
            }


            System.Type abilityType = System.Type.GetType(abilityScriptName);
            if (abilityType == null)
            {
                abilityType = System.Type.GetType(abilityScriptName + ", Assembly-CSharp");
            }

            if (abilityType == null)
            {
                Debug.LogError("Could not resolve ability type: " + abilityScriptName);
                return true;
            }

            SpecialAbility specialAbility = base.playerController.gameObject.AddComponent(abilityType) as SpecialAbility;
            if (specialAbility == null)
            {
                Debug.LogError("The type " + abilityScriptName + " is not a SpecialAbility.");
                return true;
            }


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
                GameObject go = Object.Instantiate(effect, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.parent = myTransform;
                go.transform.localPosition = localOffset;
                go.transform.localScale = Vector3.one;
            }


            System.Type abilityType = System.Type.GetType(abilityScriptName);
            if (abilityType == null)
            {

                abilityType = System.Type.GetType(abilityScriptName + ", Assembly-CSharp");
            }

            if (abilityType == null)
            {
                Debug.LogError("Could not resolve ability type: " + abilityScriptName);
                return;
            }

            SpecialAbility specialAbility = base.playerController.gameObject.AddComponent(abilityType) as SpecialAbility;
            if (specialAbility == null)
            {
                Debug.LogError("Type '" + abilityScriptName + "' is not a SpecialAbility.");
                return;
            }


            specialAbility.duration = abilityDuration - (float)delay / 1000f;
            specialAbility.effectPrefab = effect;
        }

    }
}
