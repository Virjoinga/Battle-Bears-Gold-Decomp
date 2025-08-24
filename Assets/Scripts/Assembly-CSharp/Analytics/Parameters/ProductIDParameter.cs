namespace Analytics.Parameters
{
	public class ProductIDParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.productID;
			}
		}

		public ProductIDParameter(string value)
			: base(value)
		{
		}
	}
}
