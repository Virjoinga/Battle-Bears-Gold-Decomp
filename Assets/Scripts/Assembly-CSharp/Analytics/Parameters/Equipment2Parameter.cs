namespace Analytics.Parameters
{
	public class Equipment2Parameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.equipment2;
			}
		}

		public Equipment2Parameter(Item item)
			: base(item.name)
		{
		}
	}
}
