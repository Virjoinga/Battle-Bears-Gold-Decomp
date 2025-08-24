using System.Collections.Generic;
using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class LoadoutParameters : IEventParameterEnumerable
	{
		public LoadoutParameters(PlayerLoadout loadout)
		{
			List<IEventParameter> list = new List<IEventParameter>
			{
				new LoadoutSlotParameter(loadout.loadoutNumber),
				new CharacterParameter(loadout.model),
				new SkinParameter(loadout.skin),
				new PrimaryWeaponParameter(loadout.primary),
				new SecondaryWeaponParameter(loadout.secondary),
				new MeleeWeaponParameter(loadout.melee)
			};
			if (loadout.taunt != null && !string.IsNullOrEmpty(loadout.taunt.name))
			{
				list.Add(new TauntParameter(loadout.taunt));
			}
			if (loadout.special != null && !string.IsNullOrEmpty(loadout.special.name))
			{
				list.Add(new SpecialAbilityParameter(loadout.special));
			}
			if (loadout.equipment1 != null && !string.IsNullOrEmpty(loadout.equipment1.name))
			{
				list.Add(new Equipment1Parameter(loadout.equipment1));
			}
			if (loadout.equipment2 != null && !string.IsNullOrEmpty(loadout.equipment2.name))
			{
				list.Add(new Equipment2Parameter(loadout.equipment2));
			}
			_eventParameters = list.ToArray();
		}
	}
}
