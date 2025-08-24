namespace Analytics.Parameters
{
	public class VirtualCurrencyAmountParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.virtualCurrencyAmount;
			}
		}

		public VirtualCurrencyAmountParameter(int amount)
			: base(amount)
		{
		}
	}
}
