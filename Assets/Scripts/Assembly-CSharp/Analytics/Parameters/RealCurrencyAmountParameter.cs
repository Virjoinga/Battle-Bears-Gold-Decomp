namespace Analytics.Parameters
{
	public class RealCurrencyAmountParameter : IntParameter
	{
		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.realCurrencyAmount;
			}
		}

		public RealCurrencyAmountParameter(int amount)
			: base(amount)
		{
		}
	}
}
