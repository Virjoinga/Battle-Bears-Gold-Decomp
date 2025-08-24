namespace Analytics.Parameters
{
	public class VirtualPurchaseErrorParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.virtualPurchaseError;
			}
		}

		public VirtualPurchaseErrorParameter(string value)
			: base(value)
		{
		}
	}
}
