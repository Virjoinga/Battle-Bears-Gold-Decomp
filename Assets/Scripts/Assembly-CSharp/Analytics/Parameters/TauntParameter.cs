namespace Analytics.Parameters
{
	public class TauntParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.taunt;
			}
		}

		public TauntParameter(Item item)
			: base(item.name)
		{
		}
	}
}
