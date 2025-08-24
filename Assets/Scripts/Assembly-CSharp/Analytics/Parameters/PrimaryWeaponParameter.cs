namespace Analytics.Parameters
{
	public class PrimaryWeaponParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.primaryWeapon;
			}
		}

		public PrimaryWeaponParameter(Item item)
			: base(item.name)
		{
		}
	}
}
