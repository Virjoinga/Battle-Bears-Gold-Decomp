namespace Analytics.Parameters
{
	public class SpecialAbilityParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.specialAbility;
			}
		}

		public SpecialAbilityParameter(Item item)
			: base(item.name)
		{
		}
	}
}
