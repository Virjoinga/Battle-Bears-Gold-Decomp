namespace Analytics.Parameters
{
	public class Equipment1Parameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.equipment1;
			}
		}

		public Equipment1Parameter(Item item)
			: base(item.name)
		{
		}
	}
}
