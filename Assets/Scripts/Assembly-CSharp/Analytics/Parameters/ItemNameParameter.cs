namespace Analytics.Parameters
{
	public class ItemNameParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.itemName;
			}
		}

		public ItemNameParameter(string name)
			: base(name)
		{
		}
	}
}
