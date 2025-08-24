using System.Collections.Generic;
using Analytics.Parameters.Collections;

namespace Analytics.Parameters
{
	public class KillerLoadoutParameters : IEventParameterEnumerable
	{
		public KillerLoadoutParameters(PlayerLoadout loadout)
		{
			List<IEventParameter> list = new List<IEventParameter>
			{
				new CharacterParameter(loadout.model),
				new PrimaryWeaponParameter(loadout.primary),
				new SecondaryWeaponParameter(loadout.secondary),
				new MeleeWeaponParameter(loadout.melee)
			};
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
