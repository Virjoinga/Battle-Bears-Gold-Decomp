namespace Analytics.Parameters
{
	public class IAPPurchaseErrorParameter : StringParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.iapPurchaseError;
			}
		}

		public IAPPurchaseErrorParameter(string value)
			: base(value)
		{
		}
	}
}
