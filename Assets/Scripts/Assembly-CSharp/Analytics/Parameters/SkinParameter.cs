namespace Analytics.Parameters
{
	public class SkinParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.skin;
			}
		}

		public SkinParameter(Item item)
			: base(item.name)
		{
		}
	}
}
