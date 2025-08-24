using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Item
{
	public enum Types
	{
		character = 0,
		special = 1,
		equipment = 2,
		skin = 3,
		primary = 4,
		secondary = 5,
		melee = 6,
		unlockable = 7,
		proMode = 8,
		loadout = 9,
		taunt = 10
	}

	public int id = -1;

	public string name = string.Empty;

	public string title = string.Empty;

	public string description = string.Empty;

	public int level = -1;

	public string type = string.Empty;

	public int parent_id = -1;

	public bool is_default;

	public ItemStat[] stats;

	public bool is_equippable = true;

	public bool is_allowed_for_this_version = true;

	public Dictionary<string, double> properties = new Dictionary<string, double>();

	public string ListProperties()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string key in properties.Keys)
		{
			stringBuilder.AppendFormat("{0}, ", key);
		}
		return stringBuilder.ToString();
	}

	private void ApplyModifier(string pName, string modifier, ref float pVal)
	{
		bool flag = false;
		switch (modifier)
		{
		case "ammo_1":
			flag = pName == "clipSize";
			break;
		case "ammo_2":
			flag = pName == "clipSize";
			break;
		case "armour_1":
			flag = pName == "health";
			break;
		case "armour_2":
			flag = pName == "health";
			break;
		case "damage_1":
			flag = pName == "damage";
			break;
		case "damage_2":
			flag = pName == "damage";
			break;
		case "melee_1":
			flag = pName == "melee_damage";
			break;
		case "melee_2":
			flag = pName == "melee_damage";
			break;
		case "explosion_1":
			flag = pName == "minDamage" || pName == "maxDamage";
			break;
		case "explosion_2":
			flag = pName == "minDamage" || pName == "maxDamage";
			break;
		case "speed_1":
			flag = pName == "speed_multiplier";
			break;
		case "speed_2":
			flag = pName == "speed_multiplier";
			break;
		}
		if (flag && properties.ContainsKey(modifier))
		{
			pVal += (float)properties[modifier];
		}
	}

	public bool HasBonusProperty(string pName)
	{
		return properties.ContainsKey(pName);
	}

	public float GetBonusProperty(string pName)
	{
		if (properties.ContainsKey(pName))
		{
			return (float)properties[pName];
		}
		return 0f;
	}

	public void UpdateProperty(string pName, ref float pVal, string modifiers)
	{
		if (Bootloader.Instance.permitItemModifiers)
		{
			if (modifiers == string.Empty)
			{
				Debug.LogWarning("Empty modifier string passed for property " + pName + " on item " + name + ". It should be at least '|'");
			}
			if (properties.ContainsKey(pName))
			{
				pVal = (float)properties[pName];
			}
			string[] array = modifiers.Split('|');
			if (array.Length >= 1)
			{
				ApplyModifier(pName, array[0], ref pVal);
			}
			if (array.Length >= 2)
			{
				ApplyModifier(pName, array[1], ref pVal);
			}
		}
	}

	public void UpdateProperty(string pName, ref int pVal, string modifiers)
	{
		float pVal2 = pVal;
		UpdateProperty(pName, ref pVal2, modifiers);
		pVal = (int)pVal2;
	}

	public void UpdateProperty(string pName, ref bool pVal)
	{
		if (properties.ContainsKey(pName))
		{
			pVal = properties[pName] != 0.0;
		}
	}

	public static Item Test()
	{
		Item item = new Item();
		item.id = 15;
		item.parent_id = -1;
		item.name = "Oliver";
		item.type = "Character";
		item.title = "Oliver";
		item.description = "He's a bear, his name is Oliver.";
		item.is_equippable = true;
		item.properties = new Dictionary<string, double>();
		item.properties.Add("awesomeness", 9001.0);
		item.properties.Add("times_this_worked", 0.0);
		return item;
	}
}
