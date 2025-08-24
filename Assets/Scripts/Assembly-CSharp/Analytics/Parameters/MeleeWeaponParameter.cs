namespace Analytics.Parameters
{
	public class MeleeWeaponParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.meleeWeapon;
			}
		}

		public MeleeWeaponParameter(Item item)
			: base(item.name)
		{
		}
	}
}
