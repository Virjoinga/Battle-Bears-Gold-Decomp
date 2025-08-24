namespace Analytics.Parameters
{
	public class SecondaryWeaponParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.secondaryWeapon;
			}
		}

		public SecondaryWeaponParameter(Item item)
			: base(item.name)
		{
		}
	}
}
