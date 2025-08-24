using System;

namespace Analytics.Parameters
{
	public class ItemTypeParameter : StringParameter
	{
		public enum ItemType
		{
			NONE = 0,
			LOADOUT = 1,
			CHARACTER = 2,
			SKIN = 3,
			PRIMARY_WEAPON = 4,
			SECONDARY_WEAPON = 5,
			TAUNT = 6,
			MELEE_WEAPON = 7,
			EQUIPMENT = 8,
			SPECIAL_ABILITY = 9,
			PRO_MODE_ABILITY = 10,
			UNLOCKABLE = 11,
			DEAL = 12
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.itemType;
			}
		}

		public ItemTypeParameter(Item item)
			: this(FromItem(item))
		{
		}

		public ItemTypeParameter(Gearup.GearupSubmenu submenu)
			: this(FromSubmenu(submenu))
		{
		}

		public ItemTypeParameter(ItemType type)
			: base(type.ToString())
		{
		}

		private static ItemType FromSubmenu(Gearup.GearupSubmenu submenu)
		{
			switch (submenu)
			{
			case Gearup.GearupSubmenu.LOADOUT:
				return ItemType.LOADOUT;
			case Gearup.GearupSubmenu.CHARACTER:
				return ItemType.CHARACTER;
			case Gearup.GearupSubmenu.PRIMARY:
				return ItemType.PRIMARY_WEAPON;
			case Gearup.GearupSubmenu.SECONDARY:
				return ItemType.SECONDARY_WEAPON;
			case Gearup.GearupSubmenu.MELEE:
				return ItemType.MELEE_WEAPON;
			case Gearup.GearupSubmenu.SPECIAL:
				return ItemType.SPECIAL_ABILITY;
			case Gearup.GearupSubmenu.EQUIPMENT1:
			case Gearup.GearupSubmenu.EQUIPMENT2:
				return ItemType.EQUIPMENT;
			case Gearup.GearupSubmenu.SKIN:
				return ItemType.SKIN;
			case Gearup.GearupSubmenu.PROMODE:
				return ItemType.PRO_MODE_ABILITY;
			case Gearup.GearupSubmenu.TAUNT:
				return ItemType.TAUNT;
			case Gearup.GearupSubmenu.NONE:
				return ItemType.NONE;
			default:
				throw new Exception("No ItemType for Submenu " + submenu);
			}
		}

		private static ItemType FromItem(Item item)
		{
			switch (item.type)
			{
			case "character":
				return ItemType.CHARACTER;
			case "skin":
				return ItemType.SKIN;
			case "primary":
				return ItemType.PRIMARY_WEAPON;
			case "secondary":
				return ItemType.SECONDARY_WEAPON;
			case "special":
				return ItemType.SPECIAL_ABILITY;
			case "equipment":
				return ItemType.EQUIPMENT;
			case "melee":
				return ItemType.MELEE_WEAPON;
			case "unlockable":
				return ItemType.UNLOCKABLE;
			case "proMode":
				return ItemType.PRO_MODE_ABILITY;
			case "loadout":
				return ItemType.LOADOUT;
			case "taunt":
				return ItemType.TAUNT;
			default:
				throw new Exception("No ItemType for Item's type " + item.type);
			}
		}
	}
}
