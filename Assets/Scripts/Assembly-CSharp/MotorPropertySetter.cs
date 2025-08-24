using UnityEngine;

public class MotorPropertySetter : MonoBehaviour
{
	private CharacterMotor cm;

	private float equipmentSpeedMultiplier;

	private void Start()
	{
		cm = GetComponent<CharacterMotor>();
		if (cm != null)
		{
			PlayerDamageReceiver component = GetComponent<PlayerDamageReceiver>();
			Item itemByName = ServiceManager.Instance.GetItemByName(component.itemName);
			itemByName.UpdateProperty("speed_multiplier", ref equipmentSpeedMultiplier, component.EquipmentNames);
			itemByName.UpdateProperty("forward_speed", ref cm.movement.maxForwardSpeed, component.EquipmentNames);
			itemByName.UpdateProperty("side_speed", ref cm.movement.maxSidewaysSpeed, component.EquipmentNames);
			itemByName.UpdateProperty("back_speed", ref cm.movement.maxBackwardsSpeed, component.EquipmentNames);
			itemByName.UpdateProperty("ground_acceleration", ref cm.movement.maxGroundAcceleration, component.EquipmentNames);
			itemByName.UpdateProperty("air_acceleration", ref cm.movement.maxAirAcceleration, component.EquipmentNames);
			PlayerController component2 = GetComponent<PlayerController>();
			float num = 0f;
			if (component2.CharacterManager != null)
			{
				Item skin = component2.CharacterManager.playerLoadout.skin;
				if (skin != null)
				{
					num += skin.GetBonusProperty("speedMultiplier");
				}
				Item primary = component2.CharacterManager.playerLoadout.primary;
				num += primary.GetBonusProperty("speedMultiplier");
				Item secondary = component2.CharacterManager.playerLoadout.secondary;
				num += secondary.GetBonusProperty("speedMultiplier");
				Item melee = component2.CharacterManager.playerLoadout.melee;
				num += melee.GetBonusProperty("speedMultiplier");
				Item special = component2.CharacterManager.playerLoadout.special;
				if (special != null)
				{
					num += special.GetBonusProperty("speedMultiplier");
				}
				Item equipment = component2.CharacterManager.playerLoadout.equipment1;
				if (equipment != null)
				{
					num += equipment.GetBonusProperty("speedMultiplier");
				}
				Item equipment2 = component2.CharacterManager.playerLoadout.equipment2;
				if (equipment2 != null)
				{
					num += equipment2.GetBonusProperty("speedMultiplier");
				}
			}
			num /= 100f;
			cm.movement.maxForwardSpeed *= 1f + equipmentSpeedMultiplier + num;
			cm.movement.maxSidewaysSpeed *= 1f + equipmentSpeedMultiplier + num;
			cm.movement.maxBackwardsSpeed *= 1f + equipmentSpeedMultiplier + num;
			cm.movement.maxGroundAcceleration *= 1f + equipmentSpeedMultiplier + num;
		}
		Object.Destroy(this);
	}
}
